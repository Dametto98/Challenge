-- =============================================================================
-- MOTTOMAP - SPRINT 4
-- INTEGRANTES:
-- Nome: Caike Dametto         RM: 558614
-- Nome: Guilherme Janunzzi    RM: 558461
-- =============================================================================
-- Este script contém:
-- 1. Estrutura de Tabelas e Carga de Dados  
-- 2. O Trigger de Auditoria (Objeto independente)
-- 3. O PACOTE (PACKAGE) 'PKG_MOTTOMAP' contendo as Funções e Procedures
-- 4. O novo Procedure P_EXPORTAR_DATASET_COMPLETO_JSON para gerar o JSON
-- 5. Blocos de teste atualizados para o Pacote.
-- =============================================================================

-- =============================================================================
-- SEÇÃO 0: LIMPEZA DO AMBIENTE (PARA RE-EXECUÇÃO)
-- =============================================================================
BEGIN
   EXECUTE IMMEDIATE 'DROP TRIGGER TRG_AUDITA_T_MM_MOTO';
EXCEPTION
   WHEN OTHERS THEN
      IF SQLCODE != -4080 THEN
         RAISE;
      END IF;
END;
/
BEGIN
   EXECUTE IMMEDIATE 'DROP PACKAGE PKG_MOTTOMAP';
EXCEPTION
   WHEN OTHERS THEN
      IF SQLCODE != -4043 THEN
         RAISE;
      END IF;
END;
/
-- Drop de tabelas
BEGIN
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_AUDITORIA';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_CAPTURA_IMAGEM';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_CAMERA';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_HISTORICO_POSICAO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_MOVIMENTACAO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_PROBLEMA';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_MOTO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_POSICAO_PATIO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_USUARIO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_FILIAL';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_MODELO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_STATUS_MOTO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_TIPO_PROBLEMA';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_TIPO_MOVIMENTACAO';
    EXECUTE IMMEDIATE 'DROP TABLE T_MM_STATUS_POSICAO';
EXCEPTION
   WHEN OTHERS THEN
      IF SQLCODE != -942 THEN
         RAISE;
      END IF;
END;
/
-- Drop de sequences
BEGIN
    FOR i IN (SELECT sequence_name FROM user_sequences WHERE sequence_name LIKE 'SEQ_%') LOOP
        EXECUTE IMMEDIATE 'DROP SEQUENCE ' || i.sequence_name;
    END LOOP;
EXCEPTION
   WHEN OTHERS THEN
      NULL; -- Ignora erros se a sequence não existir
END;
/
-- =============================================================================
-- SEÇÃO 1: CRIAÇÃO DA ESTRUTURA DE TABELAS
-- =============================================================================
-- 1.1 Tabelas Auxiliares (Domínios)
CREATE TABLE T_MM_STATUS_MOTO ( CD_STATUS_MOTO NUMBER(3) NOT NULL PRIMARY KEY, DS_STATUS VARCHAR2(50) NOT NULL);
CREATE TABLE T_MM_TIPO_PROBLEMA ( CD_TIPO_PROBLEMA NUMBER(3) NOT NULL PRIMARY KEY, DS_TIPO_PROBLEMA VARCHAR2(100) NOT NULL);
CREATE TABLE T_MM_TIPO_MOVIMENTACAO ( CD_TIPO_MOVIMENTACAO NUMBER(3) NOT NULL PRIMARY KEY, DS_TIPO_MOVIMENTACAO VARCHAR2(50) NOT NULL);
CREATE TABLE T_MM_STATUS_POSICAO ( CD_STATUS_POSICAO NUMBER(2) NOT NULL PRIMARY KEY, DS_STATUS_POSICAO VARCHAR2(20) NOT NULL);

-- 1.2 Tabelas Principais
CREATE TABLE T_MM_MODELO ( CD_MODELO NUMBER(8) NOT NULL PRIMARY KEY, NM_MODELO VARCHAR2(100) NOT NULL, DS_MARCA VARCHAR2(50) NOT NULL, NR_ANO NUMBER(4) NOT NULL);
CREATE TABLE T_MM_FILIAL ( CD_FILIAL NUMBER(8) NOT NULL PRIMARY KEY, NM_FILIAL VARCHAR2(150) NOT NULL, DS_ENDERECO VARCHAR2(255) NOT NULL, NM_CIDADE VARCHAR2(100) NOT NULL, SG_ESTADO VARCHAR2(2) NOT NULL, NR_LINHA NUMBER(4) NOT NULL, NR_COLUNA NUMBER(4) NOT NULL, QT_CAPACIDADE_MAX NUMBER(6) NOT NULL);
CREATE TABLE T_MM_USUARIO ( CD_USUARIO NUMBER(9) NOT NULL PRIMARY KEY, CD_FILIAL NUMBER(8) NOT NULL, NM_USUARIO VARCHAR2(150) NOT NULL, DS_EMAIL VARCHAR2(100) NOT NULL, DS_SENHA VARCHAR2(200) NOT NULL, DS_CARGO VARCHAR2(20) NOT NULL, CONSTRAINT FK_USUARIO_FILIAL FOREIGN KEY (CD_FILIAL) REFERENCES T_MM_FILIAL(CD_FILIAL));
CREATE TABLE T_MM_POSICAO_PATIO ( CD_POSICAO NUMBER(6) NOT NULL PRIMARY KEY, CD_FILIAL NUMBER(8) NOT NULL, CD_STATUS_POSICAO NUMBER(2) NOT NULL, CD_IDENTIFICACAO VARCHAR2(10) NOT NULL, NR_LINHA NUMBER(4) NOT NULL, NR_COLUNA NUMBER(4) NOT NULL, DS_AREA VARCHAR2(50) NOT NULL, CONSTRAINT FK_POSICAO_FILIAL FOREIGN KEY (CD_FILIAL) REFERENCES T_MM_FILIAL(CD_FILIAL), CONSTRAINT FK_POSICAO_STATUS FOREIGN KEY (CD_STATUS_POSICAO) REFERENCES T_MM_STATUS_POSICAO(CD_STATUS_POSICAO));
CREATE TABLE T_MM_MOTO ( CD_MOTO NUMBER(9) NOT NULL PRIMARY KEY, CD_FILIAL NUMBER(8) NOT NULL, CD_MODELO NUMBER(8) NOT NULL, CD_STATUS_MOTO NUMBER(3) NOT NULL, DS_PLACA VARCHAR2(8), DS_CHASSI VARCHAR2(17), CONSTRAINT FK_MOTO_FILIAL FOREIGN KEY (CD_FILIAL) REFERENCES T_MM_FILIAL(CD_FILIAL), CONSTRAINT FK_MOTO_MODELO FOREIGN KEY (CD_MODELO) REFERENCES T_MM_MODELO(CD_MODELO), CONSTRAINT FK_MOTO_STATUS FOREIGN KEY (CD_STATUS_MOTO) REFERENCES T_MM_STATUS_MOTO(CD_STATUS_MOTO));
CREATE TABLE T_MM_PROBLEMA ( CD_PROBLEMA NUMBER(9) NOT NULL PRIMARY KEY, CD_MOTO NUMBER(9) NOT NULL, CD_USUARIO NUMBER(9) NOT NULL, CD_TIPO_PROBLEMA NUMBER(3) NOT NULL, DS_DESCRICAO VARCHAR2(255) NOT NULL, DT_REGISTRO DATE NOT NULL, ST_RESOLVIDO NUMBER(1) NOT NULL CHECK (ST_RESOLVIDO IN (0, 1)), CONSTRAINT FK_PROBLEMA_MOTO FOREIGN KEY (CD_MOTO) REFERENCES T_MM_MOTO(CD_MOTO), CONSTRAINT FK_PROBLEMA_USUARIO FOREIGN KEY (CD_USUARIO) REFERENCES T_MM_USUARIO(CD_USUARIO), CONSTRAINT FK_PROBLEMA_TIPO FOREIGN KEY (CD_TIPO_PROBLEMA) REFERENCES T_MM_TIPO_PROBLEMA(CD_TIPO_PROBLEMA));
CREATE TABLE T_MM_MOVIMENTACAO ( CD_MOVIMENTACAO NUMBER(10) NOT NULL PRIMARY KEY, CD_USUARIO NUMBER(9) NOT NULL, CD_MOTO NUMBER(9) NOT NULL, CD_TIPO_MOVIMENTACAO NUMBER(3) NOT NULL, DT_HORA DATE NOT NULL, VL_MOVIMENTACAO NUMBER(10, 2) DEFAULT 0 NOT NULL, DS_OBSERVACOES VARCHAR2(255), CONSTRAINT FK_MOVIMENTACAO_USUARIO FOREIGN KEY (CD_USUARIO) REFERENCES T_MM_USUARIO(CD_USUARIO), CONSTRAINT FK_MOVIMENTACAO_MOTO FOREIGN KEY (CD_MOTO) REFERENCES T_MM_MOTO(CD_MOTO), CONSTRAINT FK_MOVIMENTACAO_TIPO FOREIGN KEY (CD_TIPO_MOVIMENTACAO) REFERENCES T_MM_TIPO_MOVIMENTACAO(CD_TIPO_MOVIMENTACAO));
CREATE TABLE T_MM_HISTORICO_POSICAO ( CD_HISTORICO NUMBER(10) NOT NULL PRIMARY KEY, CD_MOTO NUMBER(9) NOT NULL, CD_POSICAO NUMBER(6) NOT NULL, DT_INICIO DATE NOT NULL, DT_FIM DATE, CONSTRAINT FK_HISTORICO_MOTO FOREIGN KEY (CD_MOTO) REFERENCES T_MM_MOTO(CD_MOTO), CONSTRAINT FK_HISTORICO_POSICAO FOREIGN KEY (CD_POSICao) REFERENCES T_MM_POSICAO_PATIO(CD_POSICao));
CREATE TABLE T_MM_CAMERA ( CD_CAMERA NUMBER(6) NOT NULL PRIMARY KEY, CD_FILIAL NUMBER(8) NOT NULL, DS_MODELO_CAM VARCHAR2(100) NOT NULL, DS_LOCALIZACAO VARCHAR2(255) NOT NULL, IP_ADDRESS VARCHAR2(45), CONSTRAINT FK_CAMERA_FILIAL FOREIGN KEY (CD_FILIAL) REFERENCES T_MM_FILIAL(CD_FILIAL));
CREATE TABLE T_MM_CAPTURA_IMAGEM ( CD_CAPTURA NUMBER(12) NOT NULL PRIMARY KEY, CD_CAMERA NUMBER(6) NOT NULL, CD_POSICAO_PATIO NUMBER(6), DT_CAPTURA TIMESTAMP DEFAULT SYSTIMESTAMP NOT NULL, DS_URL_IMAGEM VARCHAR2(512) NOT NULL, ST_PROCESSADA NUMBER(1) DEFAULT 0 NOT NULL CHECK (ST_PROCESSADA IN (0,1)), CONSTRAINT FK_CAPTURA_CAMERA FOREIGN KEY (CD_CAMERA) REFERENCES T_MM_CAMERA(CD_CAMERA), CONSTRAINT FK_CAPTURA_POSICAO FOREIGN KEY (CD_POSICAO_PATIO) REFERENCES T_MM_POSICAO_PATIO(CD_POSICAO));

-- 1.3 Tabela de Auditoria (para o Trigger)
CREATE TABLE T_MM_AUDITORIA (
    CD_AUDITORIA        NUMBER(10) NOT NULL PRIMARY KEY,
    NM_USUARIO          VARCHAR2(100),
    DT_OPERACAO         TIMESTAMP,
    NM_TABELA           VARCHAR2(30),
    DS_OPERACAO         VARCHAR2(10), -- INSERT, UPDATE, DELETE
    DS_VALORES_ANTIGOS  CLOB,
    DS_VALORES_NOVOS    CLOB
);

-- 1.4 Criação de Sequences para chaves primárias
CREATE SEQUENCE SEQ_FILIAL START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_USUARIO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_POSICAO_PATIO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_MODELO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_MOTO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_PROBLEMA START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_MOVIMENTACAO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_HISTORICO_POSICAO START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_CAMERA START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_CAPTURA_IMAGEM START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE SEQ_AUDITORIA START WITH 1 INCREMENT BY 1;

-- =============================================================================
-- SEÇÃO 2: CARGA DE DADOS
-- =============================================================================
-- Tabelas Auxiliares
INSERT INTO T_MM_STATUS_MOTO VALUES (1, 'Disponível');
INSERT INTO T_MM_STATUS_MOTO VALUES (2, 'Em Manutenção');
INSERT INTO T_MM_STATUS_MOTO VALUES (3, 'Vendido');
INSERT INTO T_MM_STATUS_MOTO VALUES (4, 'Aguardando Peças');
INSERT INTO T_MM_STATUS_MOTO VALUES (5, 'Em Test-drive');
INSERT INTO T_MM_TIPO_PROBLEMA VALUES (1, 'Motor não liga');
INSERT INTO T_MM_TIPO_PROBLEMA VALUES (2, 'Sistema elétrico falhando');
INSERT INTO T_MM_TIPO_PROBLEMA VALUES (3, 'Freios com ruído');
INSERT INTO T_MM_TIPO_PROBLEMA VALUES (4, 'Pneu furado');
INSERT INTO T_MM_TIPO_PROBLEMA VALUES (5, 'Vazamento de óleo');
INSERT INTO T_MM_TIPO_MOVIMENTACAO VALUES (1, 'Entrada no Pátio');
INSERT INTO T_MM_TIPO_MOVIMENTACAO VALUES (2, 'Saída para Venda');
INSERT INTO T_MM_TIPO_MOVIMENTACAO VALUES (3, 'Envio para Oficina');
INSERT INTO T_MM_TIPO_MOVIMENTACAO VALUES (4, 'Retorno da Oficina');
INSERT INTO T_MM_TIPO_MOVIMENTACAO VALUES (5, 'Transferência entre Filiais');
INSERT INTO T_MM_STATUS_POSICAO VALUES (1, 'Livre');
INSERT INTO T_MM_STATUS_POSICAO VALUES (2, 'Ocupado');
INSERT INTO T_MM_STATUS_POSICAO VALUES (3, 'Reservado');
INSERT INTO T_MM_STATUS_POSICAO VALUES (4, 'Em Manutenção');
INSERT INTO T_MM_STATUS_POSICAO VALUES (5, 'Interditado');

-- Tabelas Principais
INSERT INTO T_MM_FILIAL VALUES (SEQ_FILIAL.NEXTVAL, 'Mottu SP', 'Av. Paulista, 1000', 'São Paulo', 'SP', 10, 20, 200);
INSERT INTO T_MM_FILIAL VALUES (SEQ_FILIAL.NEXTVAL, 'Mottu RJ', 'Av. Copacabana, 500', 'Rio de Janeiro', 'RJ', 8, 15, 120);
INSERT INTO T_MM_FILIAL VALUES (SEQ_FILIAL.NEXTVAL, 'Mottu MG', 'Av. Afonso Pena, 200', 'Belo Horizonte', 'MG', 12, 10, 120);
INSERT INTO T_MM_FILIAL VALUES (SEQ_FILIAL.NEXTVAL, 'Mottu PR', 'Rua XV de Novembro, 300', 'Curitiba', 'PR', 10, 10, 100);
INSERT INTO T_MM_FILIAL VALUES (SEQ_FILIAL.NEXTVAL, 'Mottu BA', 'Av. Oceânica, 400', 'Salvador', 'BA', 5, 20, 100);
INSERT INTO T_MM_USUARIO VALUES (SEQ_USUARIO.NEXTVAL, 1, 'Ana Silva', 'ana.silva@motomap.com', 'Senha@123', 'Gerente');
INSERT INTO T_MM_USUARIO VALUES (SEQ_USUARIO.NEXTVAL, 1, 'Bruno Costa', 'bruno.costa@motomap.com', 'Senha#456', 'Vendedor');
INSERT INTO T_MM_USUARIO VALUES (SEQ_USUARIO.NEXTVAL, 2, 'Carlos Dias', 'carlos.dias@motomap.com', 'Senha$789', 'Mecânico');
INSERT INTO T_MM_USUARIO VALUES (SEQ_USUARIO.NEXTVAL, 2, 'Daniela Souza', 'daniela.souza@motomap.com', 'Senha!101', 'Vendedor');
INSERT INTO T_MM_USUARIO VALUES (SEQ_USUARIO.NEXTVAL, 3, 'Eduardo Lima', 'eduardo.lima@motomap.com', 'Senha%112', 'Gerente');
INSERT INTO T_MM_MODELO VALUES (SEQ_MODELO.NEXTVAL, 'MOTTU SPORT ESD', 'MOTTU', 2023);
INSERT INTO T_MM_MODELO VALUES (SEQ_MODELO.NEXTVAL, 'POP 110I', 'Honda', 2024);
INSERT INTO T_MM_MODELO VALUES (SEQ_MODELO.NEXTVAL, 'MOTTU SPORT', 'MOTTU', 2022);
INSERT INTO T_MM_MODELO VALUES (SEQ_MODELO.NEXTVAL, 'MOTTU-E MAX', 'MOTTU', 2023);
INSERT INTO T_MM_MODELO VALUES (SEQ_MODELO.NEXTVAL, 'MOTTU SPORT', 'MOTTU', 2024);
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 1, 1, 1, 'BRA2E19', '9C2KC1600NR001234');
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 1, 2, 2, 'DUS3A70', '8A7YB2500PR112233');
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 2, 3, 1, 'SLK7B84', '7B6ZC3400QR223344');
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 3, 4, 4, 'RSD7J19', '6C5XD4300RS334455');
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 3, 5, 1, 'FHR4G76', '5D4WE5200ST445566');
INSERT INTO T_MM_MOTO VALUES (SEQ_MOTO.NEXTVAL, 1, 5, 3, 'SAH2D23', '4E3VF6100TU556677');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 1, 1, 1, SYSDATE - 10, 100.00, 'Entrada de lote');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 2, 1, 3, SYSDATE - 9, 250.50, 'Envio para troca de pneu');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 1, 2, 1, SYSDATE - 8, 100.00, 'Entrada de lote');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 3, 3, 5, SYSDATE - 7, 50.00, 'Transferência de RJ para SP');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 4, 4, 1, SYSDATE - 6, 120.00, 'Entrada de lote');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 5, 5, 3, SYSDATE - 5, 300.00, 'Envio para revisão geral');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 1, 6, 2, SYSDATE - 4, 15000.00, 'Venda para cliente');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 2, 1, 4, SYSDATE - 3, 50.25, 'Retorno da oficina');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 2, 2, 3, SYSDATE - 2, 450.00, 'Envio para conserto de motor');
INSERT INTO T_MM_MOVIMENTACAO VALUES (SEQ_MOVIMENTACAO.NEXTVAL, 3, 3, 1, SYSDATE - 1, 100.00, 'Entrada de lote');
INSERT INTO T_MM_POSICAO_PATIO VALUES (SEQ_POSICAO_PATIO.NEXTVAL, 1, 2, 'A01', 1, 1, 'Vendas');
INSERT INTO T_MM_POSICAO_PATIO VALUES (SEQ_POSICAO_PATIO.NEXTVAL, 1, 2, 'A02', 1, 2, 'Vendas');
INSERT INTO T_MM_POSICAO_PATIO VALUES (SEQ_POSICAO_PATIO.NEXTVAL, 1, 1, 'B01', 2, 1, 'Manutenção');
INSERT INTO T_MM_POSICAO_PATIO VALUES (SEQ_POSICAO_PATIO.NEXTVAL, 2, 2, 'A01', 1, 1, 'Vendas');
INSERT INTO T_MM_POSICAO_PATIO VALUES (SEQ_POSICAO_PATIO.NEXTVAL, 2, 1, 'A02', 1, 2, 'Vendas');
INSERT INTO T_MM_PROBLEMA VALUES (SEQ_PROBLEMA.NEXTVAL, 2, 2, 2, 'Luz do painel piscando', SYSDATE - 8, 0);
INSERT INTO T_MM_PROBLEMA VALUES (SEQ_PROBLEMA.NEXTVAL, 4, 5, 1, 'Moto falha ao tentar dar partida', SYSDATE - 5, 0);
INSERT INTO T_MM_PROBLEMA VALUES (SEQ_PROBLEMA.NEXTVAL, 5, 5, 5, 'Mancha de óleo encontrada sob a moto', SYSDATE - 4, 1);
INSERT INTO T_MM_PROBLEMA VALUES (SEQ_PROBLEMA.NEXTVAL, 1, 2, 3, 'Ruído alto ao frear com freio dianteiro', SYSDATE - 3, 0);
INSERT INTO T_MM_PROBLEMA VALUES (SEQ_PROBLEMA.NEXTVAL, 2, 3, 2, 'Setas não estão funcionando', SYSDATE - 2, 1);
INSERT INTO T_MM_HISTORICO_POSICAO VALUES (SEQ_HISTORICO_POSICAO.NEXTVAL, 1, 1, SYSDATE - 10, NULL);
INSERT INTO T_MM_HISTORICO_POSICAO VALUES (SEQ_HISTORICO_POSICAO.NEXTVAL, 2, 2, SYSDATE - 8, NULL);
INSERT INTO T_MM_HISTORICO_POSICAO VALUES (SEQ_HISTORICO_POSICAO.NEXTVAL, 3, 4, SYSDATE - 7, NULL);
INSERT INTO T_MM_HISTORICO_POSICAO VALUES (SEQ_HISTORICO_POSICAO.NEXTVAL, 4, 1, SYSDATE - 6, SYSDATE - 5);
INSERT INTO T_MM_HISTORICO_POSICAO VALUES (SEQ_HISTORICO_POSICAO.NEXTVAL, 5, 2, SYSDATE - 5, SYSDATE - 4);
INSERT INTO T_MM_CAMERA VALUES (SEQ_CAMERA.NEXTVAL, 1, 'Intelbras VHD 3230', 'Entrada Pátio Vendas', '192.168.1.101');
INSERT INTO T_MM_CAMERA VALUES (SEQ_CAMERA.NEXTVAL, 1, 'Hikvision DS-2CD2143', 'Corredor B - Manutenção', '192.168.1.102');
INSERT INTO T_MM_CAMERA VALUES (SEQ_CAMERA.NEXTVAL, 2, 'Intelbras VHD 3230', 'Entrada Principal', '192.168.2.50');
INSERT INTO T_MM_CAMERA VALUES (SEQ_CAMERA.NEXTVAL, 3, 'TP-Link Tapo C200', 'Escritório Gerência', '192.168.3.25');
INSERT INTO T_MM_CAMERA VALUES (SEQ_CAMERA.NEXTVAL, 1, 'Hikvision DS-2CD2143', 'Saída Oficina', '192.168.1.103');
INSERT INTO T_MM_CAPTURA_IMAGEM (CD_CAPTURA, CD_CAMERA, CD_POSICAO_PATIO, DS_URL_IMAGEM) VALUES (SEQ_CAPTURA_IMAGEM.NEXTVAL, 1, 1, '/img/captura/20240520_103000_cam01.jpg');
INSERT INTO T_MM_CAPTURA_IMAGEM (CD_CAPTURA, CD_CAMERA, CD_POSICAO_PATIO, DS_URL_IMAGEM) VALUES (SEQ_CAPTURA_IMAGEM.NEXTVAL, 1, 2, '/img/captura/20240520_103100_cam01.jpg');
INSERT INTO T_MM_CAPTURA_IMAGEM (CD_CAPTURA, CD_CAMERA, CD_POSICAO_PATIO, DS_URL_IMAGEM) VALUES (SEQ_CAPTURA_IMAGEM.NEXTVAL, 2, 3, '/img/captura/20240520_110000_cam02.jpg');
INSERT INTO T_MM_CAPTURA_IMAGEM (CD_CAPTURA, CD_CAMERA, CD_POSICAO_PATIO, DS_URL_IMAGEM) VALUES (SEQ_CAPTURA_IMAGEM.NEXTVAL, 3, 4, '/img/captura/20240520_120000_cam03.jpg');
INSERT INTO T_MM_CAPTURA_IMAGEM (CD_CAPTURA, CD_CAMERA, CD_POSICAO_PATIO, DS_URL_IMAGEM) VALUES (SEQ_CAPTURA_IMAGEM.NEXTVAL, 5, NULL, '/img/captura/20240520_140000_cam05.jpg');
COMMIT;

-- =============================================================================
-- SEÇÃO 3: TRIGGER DE AUDITORIA
-- =============================================================================
-- NOTA: Triggers são objetos de schema e não podem ser colocados dentro
-- de um pacote. Eles permanecem como objetos independentes.
-- -----------------------------------------------------------------------------
CREATE OR REPLACE TRIGGER TRG_AUDITA_T_MM_MOTO
AFTER INSERT OR UPDATE OR DELETE ON T_MM_MOTO
FOR EACH ROW
DECLARE
    v_operacao      VARCHAR2(10);
    v_old_values    CLOB;
    v_new_values    CLOB;
BEGIN
    IF INSERTING THEN
        v_operacao := 'INSERT';
        v_new_values := 'PLACA: ' || :NEW.DS_PLACA || '; CHASSI: ' || :NEW.DS_CHASSI || '; STATUS: ' || :NEW.CD_STATUS_MOTO;
        v_old_values := NULL;
    ELSIF UPDATING THEN
        v_operacao := 'UPDATE';
        v_old_values := 'PLACA: ' || :OLD.DS_PLACA || '; CHASSI: ' || :OLD.DS_CHASSI || '; STATUS: ' || :OLD.CD_STATUS_MOTO;
        v_new_values := 'PLACA: ' || :NEW.DS_PLACA || '; CHASSI: ' || :NEW.DS_CHASSI || '; STATUS: ' || :NEW.CD_STATUS_MOTO;
    ELSIF DELETING THEN
        v_operacao := 'DELETE';
        v_old_values := 'PLACA: ' || :OLD.DS_PLACA || '; CHASSI: ' || :OLD.DS_CHASSI || '; STATUS: ' || :OLD.CD_STATUS_MOTO;
        v_new_values := NULL;
    END IF;

    INSERT INTO T_MM_AUDITORIA (
        CD_AUDITORIA,
        NM_USUARIO,
        DT_OPERACAO,
        NM_TABELA,
        DS_OPERACAO,
        DS_VALORES_ANTIGOS,
        DS_VALORES_NOVOS
    ) VALUES (
        SEQ_AUDITORIA.NEXTVAL,
        USER,
        SYSTIMESTAMP,
        'T_MM_MOTO',
        v_operacao,
        v_old_values,
        v_new_values
    );
END TRG_AUDITA_T_MM_MOTO;
/

-- =============================================================================
-- SEÇÃO 4: EMPACOTAMENTO - PKG_MOTTOMAP
-- =============================================================================
-- 4.1: ESPECIFICAÇÃO DO PACOTE (PACKAGE SPECIFICATION)
-- -----------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE PKG_MOTTOMAP
IS
    -- Função de conversão para JSON (usada internamente e externamente)
    FUNCTION F_RELACIONAL_PARA_JSON (
        p_cursor IN SYS_REFCURSOR
    ) RETURN CLOB;

    -- Função de validação de senha
    FUNCTION F_VALIDA_COMPLEXIDADE_SENHA (
        p_senha IN VARCHAR2
    ) RETURN NUMBER; -- 1 para Válido, 0 para Inválido

    -- Procedure de relatório simples de motos
    PROCEDURE P_RELATORIO_MOTOS_JSON (
        p_cd_filial IN T_MM_FILIAL.CD_FILIAL%TYPE
    );
    
    -- Procedure de relatório agregado
    PROCEDURE P_RELATORIO_AGREGADO_MOVIMENTACOES;

    -- NOVO Procedure para exportar o DATASET completo para o MongoDB
    PROCEDURE P_EXPORTAR_DATASET_COMPLETO_JSON;

END PKG_MOTTOMAP;
/

-- -----------------------------------------------------------------------------
-- 4.2: CORPO DO PACOTE (PACKAGE BODY)
-- -----------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY PKG_MOTTOMAP
IS
    -- -------------------------------------------------------------------------
    -- Implementação da Função 1: F_RELACIONAL_PARA_JSON
    -- -------------------------------------------------------------------------
    FUNCTION F_RELACIONAL_PARA_JSON (
        p_cursor IN SYS_REFCURSOR
    ) RETURN CLOB
    IS
        v_json_clob     CLOB;
        v_cursor_id     NUMBER;
        v_col_count     INTEGER;
        v_desc_tab      DBMS_SQL.DESC_TAB;
        v_col_value     VARCHAR2(4000);
        v_row_count     NUMBER := 0;
        l_cursor        SYS_REFCURSOR;
        
        e_cursor_invalido EXCEPTION;
    BEGIN
        IF p_cursor IS NULL OR NOT p_cursor%ISOPEN THEN
            RAISE e_cursor_invalido;
        END IF;

        l_cursor := p_cursor;
        v_cursor_id := DBMS_SQL.TO_CURSOR_NUMBER(l_cursor);
        DBMS_SQL.DESCRIBE_COLUMNS(v_cursor_id, v_col_count, v_desc_tab);

        FOR i IN 1..v_col_count LOOP
            DBMS_SQL.DEFINE_COLUMN(v_cursor_id, i, v_col_value, 4000);
        END LOOP;

        v_json_clob := '[';

        WHILE DBMS_SQL.FETCH_ROWS(v_cursor_id) > 0 LOOP
            v_row_count := v_row_count + 1;
            IF v_row_count > 1 THEN
                v_json_clob := v_json_clob || ',';
            END IF;

            v_json_clob := v_json_clob || CHR(10) || '  {';
            FOR i IN 1..v_col_count LOOP
                DBMS_SQL.COLUMN_VALUE(v_cursor_id, i, v_col_value);
                
                -- Limpa caracteres de nova linha e retorno de carro
                v_col_value := REPLACE(REPLACE(v_col_value, CHR(10), ''), CHR(13), ''); 

                v_json_clob := v_json_clob || '"' || LOWER(v_desc_tab(i).col_name) || '": ';

                IF v_desc_tab(i).col_type = 2 THEN -- NUMBER
                    v_json_clob := v_json_clob || NVL(v_col_value, 'null');
                ELSIF v_desc_tab(i).col_type = 12 THEN -- DATE (ou TIMESTAMP)
                    IF v_col_value IS NULL THEN
                        v_json_clob := v_json_clob || 'null';
                    ELSE
                        -- Formata data para ISO 8601 (YYYY-MM-DD) para consistência
                        -- Se a coluna for TIMESTAMP, TO_CHAR(data, 'YYYY-MM-DD"T"HH24:MI:SS"Z"') pode ser usado
                        -- CORREÇÃO: Tentar converter a data lida como string antes de formatar
                        BEGIN
                           v_json_clob := v_json_clob || '"' || TO_CHAR(TO_DATE(v_col_value, 'DD/MM/RR HH24:MI:SS'), 'YYYY-MM-DD') || '"'; 
                        EXCEPTION
                           WHEN OTHERS THEN -- Se falhar, tenta sem hora
                              BEGIN
                                 v_json_clob := v_json_clob || '"' || TO_CHAR(TO_DATE(v_col_value, 'DD/MM/RR'), 'YYYY-MM-DD') || '"'; 
                              EXCEPTION 
                                 WHEN OTHERS THEN -- Se falhar de novo, usa o valor original como string
                                    v_json_clob := v_json_clob || '"' || v_col_value || '"';
                              END;
                        END;
                    END IF;
                ELSE -- VARCHAR2, CHAR, CLOB, etc.
                    IF v_col_value IS NULL THEN
                        v_json_clob := v_json_clob || 'null';
                    ELSE
                        -- Escapa aspas e barras invertidas para um JSON válido
                        v_col_value := REPLACE(v_col_value, '\', '\\');
                        v_col_value := REPLACE(v_col_value, '"', '\"');
                        v_json_clob := v_json_clob || '"' || v_col_value || '"';
                    END IF;
                END IF;

                IF i < v_col_count THEN
                    v_json_clob := v_json_clob || ', ';
                END IF;
            END LOOP;
            v_json_clob := v_json_clob || '}';
        END LOOP;

        v_json_clob := v_json_clob || CHR(10) || ']';
        
        IF v_row_count = 0 THEN
            RAISE NO_DATA_FOUND;
        END IF;

        RETURN v_json_clob;

    EXCEPTION
        WHEN e_cursor_invalido THEN
            DBMS_OUTPUT.PUT_LINE('ERRO: O cursor fornecido é inválido ou não está aberto.');
            RETURN '{"erro": "Cursor inválido."}';
        WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('AVISO: A consulta não retornou dados.');
            RETURN '[]';
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERRO Inesperado na conversão para JSON (' || v_desc_tab(DBMS_SQL.LAST_ERROR_POSITION).col_name || '): ' || SQLERRM);
            IF DBMS_SQL.IS_OPEN(v_cursor_id) THEN
                DBMS_SQL.CLOSE_CURSOR(v_cursor_id);
            END IF;
            RETURN '{"erro": "Ocorreu um erro inesperado."}';
    END F_RELACIONAL_PARA_JSON;

    -- -------------------------------------------------------------------------
    -- Implementação da Função 2: F_VALIDA_COMPLEXIDADE_SENHA
    -- -------------------------------------------------------------------------
    FUNCTION F_VALIDA_COMPLEXIDADE_SENHA (
        p_senha IN VARCHAR2
    ) RETURN NUMBER -- 1 para Válido, 0 para Inválido
    IS
        e_senha_nula    EXCEPTION;
        e_senha_curta   EXCEPTION;
    BEGIN
        IF p_senha IS NULL THEN
            RAISE e_senha_nula;
        END IF;

        IF LENGTH(p_senha) < 8 THEN
            RAISE e_senha_curta;
        END IF;
        
        IF NOT (REGEXP_LIKE(p_senha, '[A-Z]') AND
                REGEXP_LIKE(p_senha, '[a-z]') AND
                REGEXP_LIKE(p_senha, '[0-9]')) THEN
            RETURN 0; -- Senha inválida
        END IF;

        RETURN 1; -- Senha válida

    EXCEPTION
        WHEN e_senha_nula THEN
            DBMS_OUTPUT.PUT_LINE('ERRO: A senha não pode ser nula.');
            RETURN 0;
        WHEN e_senha_curta THEN
            DBMS_OUTPUT.PUT_LINE('ERRO: A senha deve ter no mínimo 8 caracteres.');
            RETURN 0;
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERRO Inesperado na validação da senha: ' || SQLERRM);
            RETURN 0;
    END F_VALIDA_COMPLEXIDADE_SENHA;

    -- -------------------------------------------------------------------------
    -- Implementação do Procedure 1: P_RELATORIO_MOTOS_JSON
    -- -------------------------------------------------------------------------
    PROCEDURE P_RELATORIO_MOTOS_JSON (
        p_cd_filial IN T_MM_FILIAL.CD_FILIAL%TYPE
    )
    IS
        v_cursor    SYS_REFCURSOR;
        v_json_data CLOB;
        v_count     NUMBER;
        
        e_filial_inexistente EXCEPTION;
    BEGIN
        -- Validação de entrada
        SELECT COUNT(*) INTO v_count FROM T_MM_FILIAL WHERE CD_FILIAL = p_cd_filial;
        IF v_count = 0 THEN
            RAISE e_filial_inexistente;
        END IF;

        OPEN v_cursor FOR
            SELECT
                m.CD_MOTO,
                m.DS_PLACA,
                md.NM_MODELO,
                md.DS_MARCA,
                md.NR_ANO,
                sm.DS_STATUS
            FROM
                T_MM_MOTO m
                JOIN T_MM_MODELO md ON m.CD_MODELO = md.CD_MODELO
                JOIN T_MM_STATUS_MOTO sm ON m.CD_STATUS_MOTO = sm.CD_STATUS_MOTO
            WHERE
                m.CD_FILIAL = p_cd_filial;

        v_json_data := F_RELACIONAL_PARA_JSON(v_cursor);

        DBMS_OUTPUT.PUT_LINE('Relatório de Motos em JSON para a Filial ' || p_cd_filial);
        DBMS_OUTPUT.PUT_LINE('--------------------------------------------------');
        DBMS_OUTPUT.PUT_LINE(v_json_data);

    EXCEPTION
        WHEN e_filial_inexistente THEN
            DBMS_OUTPUT.PUT_LINE('ERRO: A filial com o código ' || p_cd_filial || ' não existe.');
        WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('PROCEDURE INFO: Nenhuma moto encontrada para a filial especificada.');
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERRO Inesperado no P_RELATORIO_MOTOS_JSON: ' || SQLERRM);
    END P_RELATORIO_MOTOS_JSON;

    -- -------------------------------------------------------------------------
    -- Implementação do Procedure 2: P_RELATORIO_AGREGADO_MOVIMENTACOES
    -- -------------------------------------------------------------------------
    PROCEDURE P_RELATORIO_AGREGADO_MOVIMENTACOES
    IS
        CURSOR c_movimentacoes IS
            SELECT
                f.CD_FILIAL, 
                f.NM_FILIAL,
                m.CD_TIPO_MOVIMENTACAO,
                tm.DS_TIPO_MOVIMENTACAO,
                SUM(m.VL_MOVIMENTACAO) AS VL_SOMADO
            FROM
                T_MM_MOVIMENTACAO m
                JOIN T_MM_MOTO mo ON m.CD_MOTO = mo.CD_MOTO
                JOIN T_MM_FILIAL f ON mo.CD_FILIAL = f.CD_FILIAL
                JOIN T_MM_TIPO_MOVIMENTACAO tm ON m.CD_TIPO_MOVIMENTACAO = tm.CD_TIPO_MOVIMENTACAO
            GROUP BY
                f.CD_FILIAL, f.NM_FILIAL, 
                m.CD_TIPO_MOVIMENTACAO, tm.DS_TIPO_MOVIMENTACAO
            ORDER BY
                f.NM_FILIAL, tm.DS_TIPO_MOVIMENTACAO;

        v_subtotal      NUMBER := 0;
        v_total_geral   NUMBER := 0;
        v_filial_ant    T_MM_FILIAL.NM_FILIAL%TYPE := NULL;
        v_row_count     NUMBER := 0;

    BEGIN
        DBMS_OUTPUT.PUT_LINE(RPAD('FILIAL', 20) || ' | ' || RPAD('TIPO MOVIMENTAÇÃO', 30) || ' | ' || LPAD('VALOR SOMADO', 15));
        DBMS_OUTPUT.PUT_LINE(RPAD('-', 20, '-') || ' | ' || RPAD('-', 30, '-') || ' | ' || RPAD('-', 15, '-'));

        FOR rec IN c_movimentacoes LOOP
            v_row_count := v_row_count + 1;
            IF v_filial_ant IS NOT NULL AND rec.NM_FILIAL != v_filial_ant THEN
                DBMS_OUTPUT.PUT_LINE(RPAD('-', 20, '-') || ' | ' || RPAD('-', 30, '-') || ' | ' || RPAD('-', 15, '-'));
                DBMS_OUTPUT.PUT_LINE(RPAD('Subtotal ' || v_filial_ant, 53, ' ') || ' | ' || LPAD(TO_CHAR(v_subtotal, 'FM999,999,990.00'), 15));
                DBMS_OUTPUT.PUT_LINE('');
                v_subtotal := 0;
            END IF;

            DBMS_OUTPUT.PUT_LINE(RPAD(rec.NM_FILIAL, 20) || ' | ' || RPAD(rec.DS_TIPO_MOVIMENTACAO, 30) || ' | ' || LPAD(TO_CHAR(rec.VL_SOMADO, 'FM999,999,990.00'), 15));
            v_subtotal := v_subtotal + rec.VL_SOMADO;
            v_total_geral := v_total_geral + rec.VL_SOMADO;
            v_filial_ant := rec.NM_FILIAL;
        END LOOP;
        
        IF v_row_count = 0 THEN
            RAISE NO_DATA_FOUND;
        END IF;

        IF v_filial_ant IS NOT NULL THEN
            DBMS_OUTPUT.PUT_LINE(RPAD('-', 20, '-') || ' | ' || RPAD('-', 30, '-') || ' | ' || RPAD('-', 15, '-'));
            DBMS_OUTPUT.PUT_LINE(RPAD('Subtotal ' || v_filial_ant, 53, ' ') || ' | ' || LPAD(TO_CHAR(v_subtotal, 'FM999,999,990.00'), 15));
        END IF;

        DBMS_OUTPUT.PUT_LINE('');
        DBMS_OUTPUT.PUT_LINE(RPAD('=', 82, '='));
        DBMS_OUTPUT.PUT_LINE(RPAD('TOTAL GERAL', 53, ' ') || ' | ' || LPAD(TO_CHAR(v_total_geral, 'FM999,999,990.00'), 15));
        DBMS_OUTPUT.PUT_LINE(RPAD('=', 82, '='));

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            DBMS_OUTPUT.PUT_LINE('AVISO: Nenhuma movimentação encontrada para gerar o relatório.');
        WHEN ZERO_DIVIDE THEN
            DBMS_OUTPUT.PUT_LINE('ERRO: Tentativa de divisão por zero no relatório agregado.');
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERRO Inesperado no P_RELATORIO_AGREGADO_MOVIMENTACOES: ' || SQLERRM);
    END P_RELATORIO_AGREGADO_MOVIMENTACOES;

    -- -------------------------------------------------------------------------
    -- Implementação do Procedure 3: P_EXPORTAR_DATASET_COMPLETO_JSON 
    -- -------------------------------------------------------------------------
    PROCEDURE P_EXPORTAR_DATASET_COMPLETO_JSON
    IS
        v_cursor    SYS_REFCURSOR;
        v_json_data CLOB;
    BEGIN
        -- Este cursor une Filial, Moto, Modelo, Status e Problemas.
        -- É o "dataset" que será exportado.
        OPEN v_cursor FOR
            SELECT 
                f.CD_FILIAL,
                f.NM_FILIAL,
                f.NM_CIDADE,
                f.SG_ESTADO,
                m.CD_MOTO,
                m.DS_PLACA,
                m.DS_CHASSI,
                md.NM_MODELO,
                md.DS_MARCA,
                md.NR_ANO,
                sm.DS_STATUS,
                p.CD_PROBLEMA,
                tp.DS_TIPO_PROBLEMA,
                p.DS_DESCRICAO AS DS_PROBLEMA,
                p.DT_REGISTRO AS DT_PROBLEMA, 
                p.ST_RESOLVIDO
            FROM 
                T_MM_FILIAL f
            LEFT JOIN 
                T_MM_MOTO m ON f.CD_FILIAL = m.CD_FILIAL
            LEFT JOIN 
                T_MM_MODELO md ON m.CD_MODELO = md.CD_MODELO
            LEFT JOIN 
                T_MM_STATUS_MOTO sm ON m.CD_STATUS_MOTO = sm.CD_STATUS_MOTO
            LEFT JOIN 
                T_MM_PROBLEMA p ON m.CD_MOTO = p.CD_MOTO
            LEFT JOIN 
                T_MM_TIPO_PROBLEMA tp ON p.CD_TIPO_PROBLEMA = tp.CD_TIPO_PROBLEMA
            ORDER BY
                f.NM_FILIAL, m.DS_PLACA, p.CD_PROBLEMA;

        v_json_data := F_RELACIONAL_PARA_JSON(v_cursor);

        DBMS_OUTPUT.PUT_LINE('--- INÍCIO DO DATASET JSON PARA EXPORTAÇÃO ---');
        
        -- O DBMS_OUTPUT.PUT_LINE tem um limite de tamanho.
        -- Para CLOBs grandes, é preciso imprimir em pedaços.
        DECLARE
            v_offset NUMBER := 1;
            v_chunk_size NUMBER := 2000; -- Ajuste conforme necessário
        BEGIN
            LOOP
                EXIT WHEN v_offset > DBMS_LOB.GETLENGTH(v_json_data);
                DBMS_OUTPUT.PUT_LINE(DBMS_LOB.SUBSTR(v_json_data, v_chunk_size, v_offset));
                v_offset := v_offset + v_chunk_size;
            END LOOP;
        END;
        
        DBMS_OUTPUT.PUT_LINE('--- FIM DO DATASET JSON ---');

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            -- Se não houver dados, retorna um JSON vazio em vez de erro
             DBMS_OUTPUT.PUT_LINE('--- INÍCIO DO DATASET JSON PARA EXPORTAÇÃO ---');
             DBMS_OUTPUT.PUT_LINE('[]');
             DBMS_OUTPUT.PUT_LINE('--- FIM DO DATASET JSON ---');
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERRO Inesperado no P_EXPORTAR_DATASET_COMPLETO_JSON: ' || SQLERRM);
            -- Em caso de erro, retorna um JSON de erro
            DBMS_OUTPUT.PUT_LINE('--- INÍCIO DO DATASET JSON PARA EXPORTAÇÃO ---');
            DBMS_OUTPUT.PUT_LINE('{"erro": "Ocorreu um erro inesperado durante a exportação."}');
            DBMS_OUTPUT.PUT_LINE('--- FIM DO DATASET JSON ---');
    END P_EXPORTAR_DATASET_COMPLETO_JSON;

END PKG_MOTTOMAP;
/

-- =============================================================================
-- SEÇÃO 5: SCRIPTS DE EXECUÇÃO E DEMONSTRAÇÃO 
-- =============================================================================
-- Correção: Remover o comentário da linha abaixo
SET SERVEROUTPUT ON SIZE 1000000;

-- -----------------------------------------------------------------------------
-- Demonstração da Função 2 (via Pacote)
-- -----------------------------------------------------------------------------
DECLARE
    v_resultado NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('--- TESTANDO PKG_MOTTOMAP.F_VALIDA_COMPLEXIDADE_SENHA ---');
    v_resultado := PKG_MOTTOMAP.F_VALIDA_COMPLEXIDADE_SENHA('SenhaValida123');
    DBMS_OUTPUT.PUT_LINE('Senha "SenhaValida123" é válida? (1=Sim/0=Não): ' || v_resultado);
    
    v_resultado := PKG_MOTTOMAP.F_VALIDA_COMPLEXIDADE_SENHA('curta');
    DBMS_OUTPUT.PUT_LINE('Senha "curta" é válida? (1=Sim/0=Não): ' || v_resultado);

    -- Teste de exceção (senha curta)
    DBMS_OUTPUT.PUT_LINE('-- Testando exceção de senha curta --');
    v_resultado := PKG_MOTTOMAP.F_VALIDA_COMPLEXIDADE_SENHA('abc');
END;
/

-- -----------------------------------------------------------------------------
-- Demonstração do Procedure 1 (via Pacote)
-- -----------------------------------------------------------------------------
BEGIN
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- TESTANDO PKG_MOTTOMAP.P_RELATORIO_MOTOS_JSON ---');
    PKG_MOTTOMAP.P_RELATORIO_MOTOS_JSON(p_cd_filial => 1);
    
    -- Teste de exceção (filial inexistente)
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '-- Testando exceção de filial inexistente --');
    PKG_MOTTOMAP.P_RELATORIO_MOTOS_JSON(p_cd_filial => 999);
END;
/

-- -----------------------------------------------------------------------------
-- Demonstração do Procedure 2 (via Pacote)
-- -----------------------------------------------------------------------------
BEGIN
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- TESTANDO PKG_MOTTOMAP.P_RELATORIO_AGREGADO_MOVIMENTACOES ---');
    PKG_MOTTOMAP.P_RELATORIO_AGREGADO_MOVIMENTACOES;
END;
/

-- -----------------------------------------------------------------------------
-- Demonstração do Trigger 
-- -----------------------------------------------------------------------------
DECLARE
    v_cd_moto T_MM_MOTO.CD_MOTO%TYPE;
BEGIN
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- TESTANDO TRG_AUDITA_T_MM_MOTO ---');

    -- Teste INSERT
    DBMS_OUTPUT.PUT_LINE('-- Testando INSERT --');
    INSERT INTO T_MM_MOTO (CD_MOTO, CD_FILIAL, CD_MODELO, CD_STATUS_MOTO, DS_PLACA, DS_CHASSI)
    VALUES (SEQ_MOTO.NEXTVAL, 1, 3, 1, 'XYZ1A23', '9C2KC1600NR999999')
    RETURNING CD_MOTO INTO v_cd_moto;
    DBMS_OUTPUT.PUT_LINE('Moto ' || v_cd_moto || ' inserida.');

    -- Teste UPDATE
    DBMS_OUTPUT.PUT_LINE('-- Testando UPDATE --');
    UPDATE T_MM_MOTO SET CD_STATUS_MOTO = 2, DS_PLACA = 'XYZ5B67' WHERE CD_MOTO = v_cd_moto;
    DBMS_OUTPUT.PUT_LINE('Moto ' || v_cd_moto || ' atualizada.');

    -- Teste DELETE
    DBMS_OUTPUT.PUT_LINE('-- Testando DELETE --');
    DELETE FROM T_MM_MOTO WHERE CD_MOTO = v_cd_moto;
    DBMS_OUTPUT.PUT_LINE('Moto ' || v_cd_moto || ' deletada.');
    
    COMMIT;
END;
/
-- Verificando os resultados da auditoria
SELECT * FROM T_MM_AUDITORIA ORDER BY DT_OPERACAO DESC;

-- -----------------------------------------------------------------------------
-- Demonstração da EXPORTAÇÃO JSON 
-- -----------------------------------------------------------------------------
BEGIN
    DBMS_OUTPUT.PUT_LINE(CHR(10) || '--- TESTANDO PKG_MOTTOMAP.P_EXPORTAR_DATASET_COMPLETO_JSON ---');
    PKG_MOTTOMAP.P_EXPORTAR_DATASET_COMPLETO_JSON;
END;
/

