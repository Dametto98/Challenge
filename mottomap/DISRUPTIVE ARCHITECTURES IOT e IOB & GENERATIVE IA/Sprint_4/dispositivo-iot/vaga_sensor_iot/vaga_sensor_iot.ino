#include <WiFi.h>
#include <HTTPClient.h>

// --- [ ⚠️ 1. EDITE AS CONFIGURAÇÕES AQUI ⚠️ ] ---
const char* WIFI_SSID = "NOME_DA_SUA_REDE_WIFI";
const char* WIFI_PASSWORD = "SENHA_DA_SUA_REDE_WIFI";
const char* API_IP = "192.168.1.10"; // IP do seu PC
const int API_PORT = 5171;         
const char* JWT_TOKEN = "[SEU_TOKEN_JWT_AQUI]";

String ENDPOINT_ENTRADA = "/api/v1/Movimentacao/entrada";
String ENDPOINT_SAIDA = "/api/v1/Movimentacao/saida";

// --- Configurações da Vaga ---
int POSICAO_ID = 101;     
int USUARIO_ID_IOT = 999; 


// --- Variáveis de Estado ---
bool vagaEstaOcupada = false; // Começa vazia
int motoIdNaVaga = 0;       // Qual moto está na vaga? (se 0 = nenhuma)

// --- Funções de Simulação de Hardware ---
long simularDistanciaSensor() {
  // return 30; // Para simular ENTRADA
  // return 500; // Para simular SAÍDA
  
  if (millis() < 10000) { 
    return 500; // Simula vaga livre
  } else { 
    return 30; // Simula que a moto chegou
  }
}
int simularLeituraRfid() {
  return 1; // Simula a leitura da moto com ID 1
}
// --- Fim da Simulação ---


void setup() {
  Serial.begin(115200);
  Serial.println("Iniciando Dispositivo de Vaga (IoT) - Mottu (.NET)");
  conectarWiFi();
}

void loop() {
  long distanciaAtual = simularDistanciaSensor();

  // 1. EVENTO DE ENTRADA: A vaga estava livre e agora está ocupada
  if (distanciaAtual < 50 && !vagaEstaOcupada) {
    Serial.println("\n[EVENTO] Vaga OCUPADA.");
    
    int motoId = simularLeituraRfid();
    if (motoId > 0) {
      // Chama a função de ENTRADA (com posicaoId)
      enviarEntradaAPI(motoId, POSICAO_ID, "Entrada registrada via IoT");
      
      vagaEstaOcupada = true;
      motoIdNaVaga = motoId;
    }
  } 
  // 2. EVENTO DE SAÍDA: A vaga estava ocupada e agora está livre
  else if (distanciaAtual > 50 && vagaEstaOcupada) {
    Serial.println("\n[EVENTO] Vaga LIBERADA.");
    
    // Chama a função de SAÍDA (sem posicaoId)
    enviarSaidaAPI(motoIdNaVaga, "Saida registrada via IoT");
    
    vagaEstaOcupada = false;
    motoIdNaVaga = 0;
  }

  delay(3000); 
}

// =========================

// FUNÇÕES DE ENVIO PARA API 

// =========================

// FUNÇÃO 1: Envia a ENTRADA (com posicaoId)
void enviarEntradaAPI(int motoId, int posicaoId, String observacao) {
  if (WiFi.status() != WL_CONNECTED) { return; }

  HTTPClient http;
  String apiUrl = "http://" + String(API_IP) + ":" + String(API_PORT) + ENDPOINT_ENTRADA;
  http.begin(apiUrl);
  
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + String(JWT_TOKEN));

  // JSON de ENTRADA (com posicaoId)
  String jsonPayload = "{";
  jsonPayload += "\"motoId\": " + String(motoId) + ",";
  jsonPayload += "\"usuarioId\": " + String(USUARIO_ID_IOT) + ",";
  jsonPayload += "\"posicaoId\": " + String(posicaoId) + ","; // <-- AQUI
  jsonPayload += "\"observacoes\": \"" + observacao + "\"";
  jsonPayload += "}";

  Serial.println("Enviando ENTRADA: " + jsonPayload);
  int httpResponseCode = http.POST(jsonPayload);
  Serial.printf("API respondeu com código: %d\n", httpResponseCode);
  
  http.end();
}

// FUNÇÃO 2: Envia a SAÍDA (sem posicaoId)
void enviarSaidaAPI(int motoId, String observacao) {
  if (WiFi.status() != WL_CONNECTED) { return; }

  HTTPClient http;
  String apiUrl = "http://" + String(API_IP) + ":" + String(API_PORT) + ENDPOINT_SAIDA;
  http.begin(apiUrl);
  
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + String(JWT_TOKEN));

  // JSON de SAÍDA (sem posicaoId)
  String jsonPayload = "{";
  jsonPayload += "\"motoId\": " + String(motoId) + ",";
  jsonPayload += "\"usuarioId\": " + String(USUARIO_ID_IOT) + ",";
  jsonPayload += "\"observacoes\": \"" + observacao + "\"";
  jsonPayload += "}";

  Serial.println("Enviando SAÍDA: " + jsonPayload);
  int httpResponseCode = http.POST(jsonPayload);
  Serial.printf("API respondeu com código: %d\n", httpResponseCode);
  
  http.end();
}

void conectarWiFi() {
  Serial.print("Conectando ao WiFi...");
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConectado!");
}