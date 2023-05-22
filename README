# DatabaseConverter

O DatabaseConverter é uma aplicação de linha de comando que converte bancos de dados Microsoft Access (MDB) para o formato SQLite. Essa ferramenta permite migrar facilmente dados de bancos de dados MDB legados para um formato mais moderno e amplamente suportado.

## Requisitos

- .NET 7 SDK

## Instalação

1. Clone este repositório em sua máquina local:

   ```bash
   git clone https://github.com/seu-usuario/DatabaseConverter.git
   ```

2. Navegue até o diretório do projeto:

   ```bash
   cd DatabaseConverter
   ```

## Uso

Para converter um banco de dados MDB para SQLite, execute o seguinte comando no terminal:

```bash
dotnet run converter -i <caminho-para-arquivo.mdb> -o <caminho-para-arquivo.db>
```

Substitua `<caminho-para-arquivo.mdb>` pelo caminho e nome do arquivo de banco de dados MDB que deseja converter, e `<caminho-para-arquivo.db>` pelo caminho e nome do arquivo de banco de dados SQLite que você deseja criar.

Exemplo:

```bash
dotnet run converter -i ~/Documents/banco.mdb -o ~/Documents/banco.db
```

## Funcionalidades

- Converte todas as tabelas do banco de dados MDB para o formato SQLite, incluindo suas estruturas de colunas e dados.
- Mapeia automaticamente os tipos de dados das colunas do MDB para os tipos de dados correspondentes no SQLite.
- Suporta tipos de dados comuns, como texto, números inteiros, números decimais, datas, booleanos e blobs.
- Mantém a integridade dos dados durante o processo de conversão.

## Contribuição

Contribuições são bem-vindas! Se você encontrar algum problema ou tiver alguma ideia de melhoria, sinta-se à vontade para abrir uma issue ou enviar um pull request.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
