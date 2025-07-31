






# InvestigalA

<img width="1872" height="918" alt="image" src="https://github.com/user-attachments/assets/ceb4df63-847b-46d9-bd64-e7d205866f14" />



## Um jogo de detetive interativo com inteligência artificial

InvestigalA é um jogo de detetive interativo que combina tecnologias web com inteligência artificial para criar uma experiência de investigação imersiva. Os jogadores assumem o papel de um detetive que investiga um caso de assassinato, com cenários, personagens e diálogos gerados por IA que respondem dinamicamente às ações do jogador. O nome do projeto é uma junção de "Investigação" e "IA" (Inteligência Artificial), refletindo seu conceito central.

<img width="1868" height="915" alt="image" src="https://github.com/user-attachments/assets/abb36539-c72b-48d1-88e6-5bd32ebdc677" />


---

## 🚀 Visão Geral do Projeto

Para cada partida, o jogo gera um cenário de crime único com múltiplos suspeitos, sendo um deles o verdadeiro culpado. Através de interrogatórios e coleta de provas, os jogadores devem analisar as respostas dos suspeitos, observar seus níveis de estresse e completar objetivos para identificar o assassino.

### Principais Funcionalidades

* Cenários de crime e personagens gerados por IA.
* Diálogos interativos em tempo real com suspeitos controlados por IA.
* Sistema dinâmico de estresse que afeta o comportamento dos suspeitos.
* Jogabilidade baseada em objetivos.
* Capacidades multiplayer em tempo real através do SignalR.

<img width="996" height="903" alt="image" src="https://github.com/user-attachments/assets/95fd0d52-a75e-43a4-9d93-41da20f4968c" />

---

## 🛠️ Arquitetura do Sistema

O projeto segue uma arquitetura cliente-servidor, composta pelos seguintes elementos:

* **Backend**:
    * **InvestigalA**: Uma biblioteca .NET Core que contém a lógica central do jogo, a integração com serviços de IA e o gerenciamento do estado do jogo.
    * **webAPI**: Uma API web ASP.NET Core que expõe endpoints para o gerenciamento do jogo e hospeda um hub SignalR para comunicação em tempo real.

* **Frontend**:
    * **webapp**: Uma aplicação web baseada em React e construída com Vite, que fornece a interface do usuário e a experiência de jogo.

* **Serviços**:
    * **Serviços de IA**: Integração com a IA Gemini do Google e com a OpenAI para gerar conteúdo de jogo e gerenciar interações.
    * **SignalR**: Para comunicação em tempo real entre o servidor e os clientes.

---

## ⚙️ Componentes do Backend

### Lógica do Jogo

A lógica central do jogo reside no projeto InvestigalA, especificamente na classe `GameInstance`. Cada instância representa uma sessão de jogo separada com seu próprio estado, suspeitos e progresso.

**Classes Principais:**

* `GameManager`: Gerencia a criação e o acesso às instâncias de jogo.
* `GameInstance`: Contém a lógica central do jogo, interação com IA e gerenciamento de estado.
* `Suspeito`: Representa um personagem no jogo que pode ser interrogado.
* `Culprit`: Uma especialização de `Suspeito` que é o verdadeiro assassino.
* `CaseFile`: Contém os detalhes sobre o caso de assassinato.

### Comunicação em Tempo Real e Threads

A comunicação em tempo real é gerenciada pelo **SignalR**. O jogo também utiliza múltiplas threads para lidar com operações concorrentes, como um temporizador de jogo e um sistema dinâmico de balanceamento de estresse para os suspeitos. O `SemaphoreSlim` é usado para garantir a segurança ao acessar recursos compartilhados.

---

## 🖥️ Componentes do Frontend

O frontend é construído com **React** e utiliza componentes do **Ant Design** para os elementos da UI.

* **Gerenciamento de Estado**: É feito através dos hooks `useState` e `useEffect` do React. O estado do jogo é sincronizado com o servidor através de chamadas de API e mensagens do SignalR.
* **Comunicação com a API**: A comunicação com o backend ocorre através de `axios` para requisições HTTP e uma conexão **SignalR** para atualizações em tempo real.

<img width="1875" height="920" alt="image" src="https://github.com/user-attachments/assets/f0022553-780c-4d8b-bc30-724e08afb578" />

---

## 🤖 Integração com IA

O coração do InvestigalA está na sua integração com IA, que alimenta os personagens e interações dinâmicas. O sistema é projetado para ser flexível, permitindo o uso de diferentes modelos de IA, como o **Gemini** do Google (principal) e o **OpenAI** (alternativo).

---

## ☁️ Implantação

A aplicação é implantada em duas partes, ambas hospedadas no **Azure**:

1.  **Backend**: Hospedado no Azure como um serviço web.
2.  **Frontend**: Implantado como uma aplicação web estática no Azure.

