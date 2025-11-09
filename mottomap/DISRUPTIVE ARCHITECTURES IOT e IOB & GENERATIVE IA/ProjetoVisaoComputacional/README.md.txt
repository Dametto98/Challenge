# MotoMap: Detecção Inteligente de Vagas em Pátios com Visão Computacional

**Projeto:** Componente de Visão Computacional para o Sistema MotoMap (Challenge FIAP 2025 - Mottu)
**Autores:** Caike Dametto e Guilherme Janunzzi

## 📜 Descrição Geral

Este projeto foca no desenvolvimento de um sistema inteligente para detectar a ocupação de vagas em pátios de motocicletas da Mottu, utilizando técnicas de visão computacional e análise de dados. O objetivo é otimizar a gestão do espaço, agilizar a alocação de veículos e fornecer informações em tempo real sobre a disponibilidade de vagas.

A dor central abordada é a dificuldade no controle físico e visual eficiente das vagas de estacionamento, levando a possíveis ineficiências na alocação de veículos e na utilização do espaço do pátio.

## 🎯 Foco Atual (Detalhado no Notebook `MotoMapDetection.ipynb`)

A etapa atual, documentada no notebook `notebooks/MotoMapDetection.ipynb`, concentra-se na:
* Análise exploratória dos metadados do dataset CNRPark-EXT.
* Limpeza e pré-processamento desses dados.
* Desenvolvimento de modelos de Machine Learning (Regressão Logística e RandomForest Classifier) para prever a ocupação das vagas com base nos metadados tabulares.
* Estabelecimento de um baseline de performance e insights para o futuro modelo de visão computacional.

## 📚 Dataset Utilizado

* **Nome:** "CNRPark-EXT Dataset"
* **Arquivo de Metadados:** `CNRPark+EXT.csv`
* **Imagens:** Associadas ao dataset (referenciadas pela coluna `image_url` no CSV).
* **Link para o Dataset:** [http://cnrpark.it/](http://cnrpark.it/)

## 🛠️ Tecnologias e Bibliotecas Utilizadas

* Python 3.x
* Pandas
* NumPy
* Matplotlib
* Seaborn
* Scikit-learn
* (Futuramente para Visão Computacional: TensorFlow/Keras ou PyTorch, OpenCV)

## ⚙️ Como Executar o Notebook

1.  **Clone o repositório:**
    ```bash
    git clone [URL_DO_SEU_REPOSITORIO]
    cd MotoMap_ChallengeFIAP
    ```
2.  **Crie um ambiente virtual (recomendado):**
    ```bash
    python -m venv venv
    source venv/bin/activate  # No Windows: venv\Scripts\activate
    ```
3.  **Instale as dependências:**
    ```bash
    pip install -r requirements.txt
    ```
    *(Nota: Crie um arquivo `requirements.txt` com `pip freeze > requirements.txt` a partir do seu ambiente de desenvolvimento)*
4.  **Baixe o dataset:**
    * Coloque o arquivo `CNRPark+EXT.csv` na pasta `data/`.
    * (Para a parte de visão computacional, as imagens associadas também precisarão ser baixadas e organizadas conforme a coluna `image_url`).
5.  **Abra o Jupyter Notebook ou Jupyter Lab:**
    ```bash
    jupyter notebook
    # ou
    jupyter lab
    ```
6.  Navegue até a pasta `notebooks/` e abra o `MotoMapDetection.ipynb`.
7.  Execute as células do notebook.

## 📈 Resultados Atuais (Baseado em Metadados)

* **Análise Exploratória:** Identificou padrões de ocupação e a distribuição da variável alvo (55.65% Ocupada, 44.35% Livre).
* **Modelo de Baseline (Regressão Logística):** Acurácia de 67.61%.
* **Modelo Melhorado (RandomForestClassifier):** Acurácia de **89.19%**.

Estes resultados indicam que os metadados por si só já possuem um bom poder preditivo para a ocupação das vagas.

## 🚀 Próximos Passos (Foco em Visão Computacional)

1.  **Correção da Conversão da Feature `datetime`:** Para permitir uma engenharia de features temporais mais precisa.
2.  **Desenvolvimento do Modelo de Visão Computacional:**
    * Pré-processamento das imagens.
    * Treinamento de uma CNN (ex: usando arquiteturas como YOLO, SSD, ou customizadas) para classificar vagas como livres/ocupadas diretamente das imagens.
    * Vinculação das predições do modelo de CV com os metadados e o modelo tabular para enriquecimento ou validação.
3.  **Criação de uma API:** Para servir o modelo e permitir a integração com o aplicativo MotoMap.
4.  **Testes em Ambiente Simulado/Real:** Validar a solução em condições próximas às reais de um pátio da Mottu.

## 🤝 Contribuições

[Se aplicável, como outras pessoas podem contribuir para o projeto. Para um projeto de faculdade, isso pode não ser necessário.]

## 📄 Pitch
https://youtu.be/VtdsDGwazFI?si=RaK5dR4x30QLxcNL

