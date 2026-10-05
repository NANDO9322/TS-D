# TS-D

Aplicação em C# para cálculo de comissões, movimentação de estoque e cálculo de multa por atraso.

A aplicação usa .NET 8 e mantém as regras de negócio separadas da interação com o terminal, deixando o código simples de testar, entender e evoluir.

## Como executar

É necessário ter o SDK do .NET 8 (ou uma versão mais recente compatível) instalado.

```bash
dotnet run --project src/TS-D
```

O programa exibe um menu com as três funcionalidades. Os arquivos JSON ficam em `src/TS-D/Data` e são copiados automaticamente para a pasta de execução.

## Testes

```bash
dotnet test
```

Os testes cobrem as faixas de comissão, incluindo os limites de R$ 100 e R$ 500; entradas e saídas de estoque; bloqueio de saldo negativo; e cálculo da multa para títulos vencidos, a vencer ou com vencimento no dia atual.

## Regras consideradas

### Comissão

- Venda abaixo de R$ 100,00: sem comissão.
- Venda de R$ 100,00 até R$ 499,99: 1%.
- Venda a partir de R$ 500,00: 5%.
- A comissão é calculada por venda e depois somada por vendedor.

Com os dados do enunciado, o resultado é:

- Ana Lima: R$ 404,99
- Carlos Oliveira: R$ 379,38
- João Silva: R$ 495,69
- Maria Souza: R$ 465,96

### Estoque

Cada movimentação recebe um identificador único (`Guid`), tipo, descrição, quantidade, data e saldo final. O programa não aceita quantidade igual ou menor que zero, produto inexistente, descrição vazia ou saída maior que o estoque disponível.

As movimentações são mantidas em memória durante a execução. Persistência em banco não foi incluída porque o enunciado fornece os dados em JSON e não solicita armazenamento permanente.

### Multa

Considerei multa simples de 2,5% por dia de atraso:

```text
multa = valor original × 0,025 × dias em atraso
```

Não há multa quando a data de vencimento é hoje ou uma data futura. Além do valor da multa, o programa mostra o total atualizado.

## Estrutura

```text
src/TS-D
├── Data            # JSONs do enunciado
├── Infrastructure  # Leitura dos arquivos
├── Models          # Objetos usados pelas regras
├── Services        # Regras de negócio
└── Program.cs      # Menu e entrada de dados

tests/TS-D.Tests
└── Testes automatizados das três regras
```
