# InvestigaIA

## Sobre o Projeto

InvestigaIA é um jogo de detetive interativo em console, onde personagens são gerados por IA e o jogador pode interagir, interrogar e investigar suspeitos para resolver um mistério.

## Como Executar o Projeto

1. **Pré-requisitos:**

   - .NET 8.0 SDK ou superior instalado ([download aqui](https://dotnet.microsoft.com/download))

2. **Clone o repositório:**

   ```sh
   git clone <url-do-repositorio>
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

   > **Importante:** Substitua `SUA_CHAVE_DE_API_AQUI` pela sua chave de API válida.

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
