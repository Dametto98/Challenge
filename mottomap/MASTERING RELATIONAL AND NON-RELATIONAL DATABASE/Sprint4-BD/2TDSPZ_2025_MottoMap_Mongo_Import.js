/*
=============================================================================
 MOTTOMAP - SPRINT 4
 Script de Importação e Estruturação MongoDB
=============================================================================
 COMO USAR:
 1. Execute o script 'MottoMap_Sprint4.sql' no seu banco Oracle.
 2. No final da execução, execute o bloco "TESTANDO PKG_MOTTOMAP.P_EXPORTAR_DATASET_COMPLETO_JSON".
 3. Copie TODO o JSON que é impresso no console (entre as linhas "INÍCIO" e "FIM").
 4. Cole o JSON copiado dentro da variável 'dadosRelacionaisJSON' abaixo (substituindo o array vazio []).
 5. Abra o terminal do MongoDB (mongosh) e execute este script.
    Ex: mongosh "mongodb+srv://..." --file 2TDSPZ_2025_MottoMap_Mongo_Import.js
=============================================================================
*/

// PASSO 1: Cole o JSON gerado pelo Oracle aqui.
const dadosRelacionaisJSON = [
  // COLE SEU JSON AQUI
  // Exemplo (cole o resultado real):
  // { "cd_filial": 1, "nm_filial": "Mottu SP", ..., "cd_moto": 1, ..., "cd_problema": 4, ... },
  // { "cd_filial": 1, "nm_filial": "Mottu SP", ..., "cd_moto": 1, ..., "cd_problema": 5, ... },
  // { "cd_filial": 1, "nm_filial": "Mottu SP", ..., "cd_moto": 2, ..., "cd_problema": 1, ... }
];

if (dadosRelacionaisJSON.length === 0) {
  print("ERRO: A variável 'dadosRelacionaisJSON' está vazia. Cole o JSON do Oracle nela antes de executar.");
} else {
  
  print("Iniciando a transformação de dados Relacional -> NoSQL (Documento)...");

  // Estrutura não-relacional (aninhada)
  const filiaisMap = new Map();

  dadosRelacionaisJSON.forEach(item => {
    // 1. Processa a Filial (nível mais alto)
    if (!filiaisMap.has(item.cd_filial)) {
      filiaisMap.set(item.cd_filial, {
        _id: item.cd_filial,
        nomeFilial: item.nm_filial,
        cidade: item.nm_cidade,
        estado: item.sg_estado,
        motos: new Map() // Usamos um Map para evitar duplicatas de motos
      });
    }

    const filial = filiaisMap.get(item.cd_filial);

    // 2. Processa a Moto (aninhada na filial)
    if (item.cd_moto && !filial.motos.has(item.cd_moto)) {
      filial.motos.set(item.cd_moto, {
        _id: item.cd_moto,
        placa: item.ds_placa,
        chassi: item.ds_chassi,
        modelo: {
          nome: item.nm_modelo,
          marca: item.ds_marca,
          ano: item.nr_ano
        },
        status: item.ds_status,
        problemas: [] // Problemas são aninhados na moto
      });
    }

    // 3. Processa o Problema (aninhado na moto)
    if (item.cd_moto && item.cd_problema) {
      const moto = filial.motos.get(item.cd_moto);
      if (moto) { // Garante que a moto existe
        moto.problemas.push({
          _id: item.cd_problema,
          tipoProblema: item.ds_tipo_problema,
          descricao: item.ds_problema,
          dataRegistro: item.dt_problema, // O Oracle já deve formatar como string
          resolvido: item.st_resolvido === 1
        });
      }
    }
  });

  // Converte os Maps internos para Arrays para o MongoDB
  const filiaisParaImportar = Array.from(filiaisMap.values()).map(filial => {
    filial.motos = Array.from(filial.motos.values());
    return filial;
  });

  print(`Dados transformados. ${filiaisParaImportar.length} documentos de filiais prontos para importação.`);

  // PASSO 2: Conexão e Importação no MongoDB
  const dbName = 'mottumap_sprint4';
  const collectionName = 'filiais';

  // Alterne para o banco de dados
  db = db.getSiblingDB(dbName);
  print(`Conectado ao banco: ${dbName}`);

  // Limpa a coleção antiga
  db.getCollection(collectionName).drop();
  print(`Coleção antiga '${collectionName}' removida.`);

  // Insere os novos dados
  try {
    const resultado = db.getCollection(collectionName).insertMany(filiaisParaImportar);
    print("--------------------------------------------------");
    print("IMPORTAÇÃO PARA O MONGODB CONCLUÍDA COM SUCESSO!");
    print(`- Documentos inseridos: ${resultado.insertedIds.length}`);
    print(`- Coleção: ${collectionName}`);
    print(`- Banco: ${dbName}`);
    print("--------------------------------------------------");
    print("Estrutura do documento no MongoDB:");
    printjson(db.getCollection(collectionName).findOne());
  } catch (e) {
    print("ERRO DURANTE A IMPORTAÇÃO PARA O MONGODB:");
    print(e);
  }
}
