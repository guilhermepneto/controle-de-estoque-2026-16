# Controle de Produtos

## Projeto

Desenvolvido durante o curso Backend da [Academia do Programador](https://www.academiadoprogramador.net) 2026, como projeto final do módulo 2: Orientação à Objetos.

## Funcionalidades

### 1. Módulo de Fornecedores

**Requisitos Funcionais**

- O sistema deve permitir registrar novos fornecedores
- O sistema deve permitir visualizar todos os fornecedores cadastrados
- O sistema deve permitir editar fornecedores existentes
- O sistema deve permitir excluir fornecedores cadastrados

**Regras de Negócio**

Campos obrigatórios:

- Nome (3-100 caracteres)
- Telefone (formatos válidos)
- CNPJ (14 dígitos)

> O sistema não deve permitir cadastro de fornecedores com mesmo CNPJ

---

### 2. Módulo de Clientes

**Requisitos Funcionais**

- O sistema deve permitir registrar novos clientes
- O sistema deve permitir visualizar todos os clientes cadastrados
- O sistema deve permitir editar clientes existentes
- O sistema deve permitir excluir clientes cadastrados

**Regras de Negócio**

Campos obrigatórios:

- Nome (3-100 caracteres)
- Telefone (formatos válidos: (XX) XXXX-XXXX ou (XX) XXXXX-XXXX)
- E-mail (15 dígitos)
- CPF (11 dígitos)

> O sistema não deve permitir cadastro de clientes com mesmo e-mail

---

### 3. Módulo de Produtos

**Requisitos Funcionais**

- O sistema deve permitir registrar novos produtos
- O sistema deve permitir visualizar todos os produtos cadastrados
- O sistema deve permitir editar produtos existentes
- O sistema deve permitir excluir produtos cadastrados

**Regras de Negócio**

Campos obrigatórios:

- Nome (3-100 caracteres)
- Descrição (5-255 caracteres)
- Quantidade em estoque (número positivo)
- Fornecedor

> O sistema deve destacar produtos com menos de 20 unidades como "em falta"

> O sistema deve atualizar a quantidade quando o produto já estiver cadastrado

---

### 4. Módulo de Funcionários

**Requisitos Funcionais**

- O sistema deve permitir registrar novos funcionários
- O sistema deve permitir visualizar todos os funcionários cadastrados
- O sistema deve permitir editar funcionários existentes
- O sistema deve permitir excluir funcionários cadastrados

**Regras de Negócio**

Campos obrigatórios:

- Nome (3-100 caracteres)
- Telefone (formatos válidos)
- CPF (11 dígitos)

> O sistema não deve permitir cadastro de funcionários com mesmo CPF

---

### 5. Módulo de Estoque

#### 5.1 Requisições de Entrada

**Requisitos Funcionais para Requisições de Entrada**

- O sistema deve permitir registrar novas requisições de entrada
- O sistema deve permitir visualizar todas as requisições de entrada

**Regras de Negócio para Requisições de Entrada**

Campos obrigatórios:

- Data (válida)
- Produto (seleção obrigatória)
- Funcionário (seleção obrigatória)
- Quantidade (número positivo)

> O sistema deve atualizar o estoque ao registrar a requisição de entrada

---

#### 5.2 Requisições de Saída

**Requisitos Funcionais para Requisições de Saída**

- O sistema deve permitir registrar novas requisições de saída
- O sistema deve permitir visualizar todas as requisições de saída

**Regras de Negócio para Requisições de Saída**

Campos obrigatórios:

- Data (válida)
- Cliente (seleção obrigatória)
- Produtos Requisitados (seleção obrigatória)

> O sistema não deve permitir requisição que exceda o estoque disponível

> O sistema deve subtrair a quantidade do estoque ao registrar a requisição

---

## Como utilizar

1. Clone o repositório ou baixe o código fonte.
2. Abra o terminal ou o prompt de comando e navegue até a pasta raiz
3. Utilize o comando abaixo para restaurar as dependências do projeto.

   ```bash
   dotnet restore
   ```

4. Para executar o projeto compilando em tempo real

   ```bash
   dotnet run --project ControleDeEstoque.WebApp
   ```

## Requisitos

- .NET 10.0 SDK
