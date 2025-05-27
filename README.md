# InvestigaIA

## Sobre o Projeto

InvestigaIA é um jogo de detetive interativo em console, onde personagens são gerados por IA e o jogador pode interagir, interrogar e investigar suspeitos para resolver um mistério.

## Como Executar o Projeto

1. **Pré-requisitos:**

   - .NET 8.0 SDK ou superior instalado ([download aqui](https://dotnet.microsoft.com/download))

2. **Clone o repositório:**

   ```sh
   git clone https://github.com/RaphaelLuizPH/Projeto-Sistemas-Operacionais-IA.git
   cd "Projeto Sistemas Operacionais IA/InvestigaIA"
   ```

3. **Crie o arquivo de configuração `appsettings.json`:**
   O arquivo deve estar na mesma pasta que o `Program.cs` (raiz do projeto `InvestigaIA`).

   Exemplo de conteúdo:

   ```json
   {
     "APIKey": "SUA_CHAVE_DE_API_AQUI",
     "APIUrl": "https://generativelanguage.googleapis.com"
   }
   ```

   > **Importante:** Substitua `SUA_CHAVE_DE_API_AQUI` pela sua chave de API válida. [Consiga uma aqui](https://aistudio.google.com/welcome?utm_source=google&utm_medium=cpc&utm_campaign=FY25-global-DR-gsem-BKWS-1710442&utm_content=text-ad-none-any-DEV_c-CRE_726176647106-ADGP_Hybrid%20%7C%20BKWS%20-%20EXA%20%7C%20Txt-Gemini-Gemini%20API%20Key-KWID_43700081658555794-kwd-2337564139625&utm_term=KW_gemini%20api%20key-ST_gemini%20api%20key&gclsrc=aw.ds&gad_source=1&gad_campaignid=21026872772&gclid=Cj0KCQjwotDBBhCQARIsAG5pinPW_SVFYd6jm6U5NTpMqJJzM0DblJxq6kI6y9MT_tQcGMPwcAzcjukaAmxhEALw_wcB).

4. **Garanta que o arquivo `appsettings.json` será copiado para a pasta de saída:**

   - No Visual Studio: clique com o botão direito no arquivo, vá em "Propriedades" e defina "Copiar para o diretório de saída" como "Sempre" ou "Se mais recente".
   - Ou adicione ao seu `.csproj`:
     ```xml
     <ItemGroup>
       <None Update="appsettings.json">
         <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
       </None>
     </ItemGroup>
     ```

5. **Rode o projeto:**
   ```sh
   dotnet run
   ```

## Como funciona o `appsettings.json`

O arquivo `appsettings.json` armazena as configurações sensíveis do projeto, como a chave da API e a URL do serviço. O programa lê essas configurações automaticamente ao iniciar.

Exemplo de `appsettings.json`:

```json
{
  "APIKey": "SUA_CHAVE_DE_API_AQUI",
  "APIUrl": "https://generativelanguage.googleapis.com"
}
```

## Observações

- Não compartilhe sua chave de API publicamente.
- O projeto utiliza Spectre.Console para a interface de console.
- Certifique-se de que todas as dependências NuGet estejam restauradas (`dotnet restore`).

---

Se tiver dúvidas, consulte o código-fonte ou abra uma issue.
