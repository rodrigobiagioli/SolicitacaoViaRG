-----------------------------------------
Projeto: SolicitacaoViaRG
-----------------------------------------

Este repositório contém três serviços principais: API, Worker e Publisher, todos construídos com .NET 6.0 e configurados para serem executados em contêineres Docker. 
O Publisher envia protocolos para a fila RabbitMQ a partir de um arquivo JSON.
O Worker processa as mensagens da fila RabbitMQ e valida suas informaçoes, armazenando os dados válidos no MongoDB e recusando os dados inválidos.
A API fornece um endpoint para consulta de protocolos incluidos no banco pelo Worker.

Pré-requisitos:
.NET 6.0 SDK
Docker
Docker Compose

--------------------------------------------
Configuração do Ambiente de Desenvolvimento:
--------------------------------------------

-Clone o repositório:
git clone https://github.com/your-username/SolicitacaoViaRG.git
cd SolicitacaoViaRG

-------------------------------
Executando o Projeto Localmente
-------------------------------

------------API -------------------
-Navegue até o diretório da API:
cd SolicitacaoViaRG.API

-Restaure as dependências:
dotnet restore

Compile e execute a API:
dotnet run

A API estará disponível em http://localhost:5000/swagger.
A API esta protegida por uma APIKEY que deve ser informada no cabeçalho das chamadas dos EndPoints, o Token esta definido no appsettings.json na variavel "ApiKey" 
e para publicação no docker-compose esta definida na variavel de ambiente AuthSettings__ApiKey.

------------WORKER-----------
Navegue até o diretório do Worker:
cd SolicitacaoViaRG.Worker

- Restaure as dependências:
dotnet restore

- Compile e execute o Worker:
dotnet run

------------PUBLISHER ---------------------------

- Navegue até o diretório do Publisher:
cd SolicitacaoViaRG.Publisher

- Restaure as dependências:
dotnet restore

- Compile e execute o Publisher:
dotnet run

------------------------------------------
Executando com Docker
------------------------------------------
Certifique-se de que o Docker e o Docker Compose estão instalados em sua máquina.

- Construa e inicie os contêineres:
docker-compose up --build

- Verifique os logs dos contêineres para garantir que todos os serviços estão funcionando corretamente:
docker-compose logs

A API estará disponível em http://localhost:5230/swagger.

------------------------------------------------------------
Estrutura do Repositório
------------------------------------------------------------
SolicitacaoViaRG.API: Contém a API para consulta de protocolos.
SolicitacaoViaRG.Worker: Contém o serviço Worker que processa mensagens da fila RabbitMQ e armazena os dados no MongoDB.
SolicitacaoViaRG.Publisher: Contém o serviço Publisher que envia protocolos para a fila RabbitMQ.
docker-compose.yml: Arquivo Docker Compose para orquestrar os serviços.
Dockerfile.api: Dockerfile para construir a imagem da API.
Dockerfile.worker: Dockerfile para construir a imagem do Worker.
Dockerfile.publisher: Dockerfile para construir a imagem do Publisher.
protocolos.json: Arquivo contendo protocolos para serem enviados pelo Publisher.

------------------------------------------------------------
Informações Importantes
------------------------------------------------------------
RabbitMQ Management: A interface de gerenciamento do RabbitMQ pode ser acessada em http://localhost:15672 com as credenciais root / 123.
MongoDB: O MongoDB é configurado para rodar na porta 27017 com as credenciais root / 123.

Diretório de Imagens: As imagens são armazenadas no diretório configurado na variavel ImageDirectory, que esta localizada nos arquivos appsettings.json para ambiente local 
e como Variaveis de ambiente no arquivo docker-compose.yml

---------------------------------
Problemas Comuns
---------------------------------

Falha ao conectar ao RabbitMQ ou MongoDB:

- Verifique se os contêineres estão em execução e saudáveis:
docker-compose ps

- Erro ao acessar a API:
 
Verifique os logs do contêiner da API:
docker-compose logs solicitacaoviarg_api

