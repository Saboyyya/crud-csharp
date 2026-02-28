using Microsoft.Data.Sqlite;

// Configuração inicial
string connectionString = "Data Source=consumidores.db";

// Criar a tabela se não existir
using (var conexao = new SqliteConnection(connectionString))
{
    conexao.Open();
    string sqlCreateTable = """
    CREATE TABLE IF NOT EXISTS Consumidores (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Nome TEXT NOT NULL,
        Idade INTEGER NOT NULL,
        Email TEXT NOT NULL
    );
    """;
    using var command = conexao.CreateCommand();
    command.CommandText = sqlCreateTable;
    command.ExecuteNonQuery();
}

// Loop do Menu Principal
bool executando = true;
while (executando)
{
    Console.WriteLine("\n--- CRUD CONSUMIDORES ---");
    Console.WriteLine("1-Adicionar | 2-Ler | 3-Atualizar | 4-Remover | 0-Sair");
    Console.Write("Digite a opção escolhida: ");
    
    if (!int.TryParse(Console.ReadLine(), out int escolha)) continue;

    switch (escolha)
    {
        case 1: Adicionar(); break;
        case 2: Ler(); break;
        case 3: Atualizar(); break;
        case 4: Remover(); break;
        case 0: executando = false; break;
        default: Console.WriteLine("Opção inválida!"); break;
    }
}

// --- MÉTODOS ---

void Adicionar()
{
    Console.Write("Nome: ");
    string nome = Console.ReadLine();
    Console.Write("Idade: ");
    int idade = int.Parse(Console.ReadLine());
    Console.Write("Email: ");
    string email = Console.ReadLine();

    using var conexao = new SqliteConnection(connectionString);
    conexao.Open();
    var command = conexao.CreateCommand();
    command.CommandText = "INSERT INTO Consumidores (Nome, Idade, Email) VALUES ($nome, $idade, $email)";
    command.Parameters.AddWithValue("$nome", nome);
    command.Parameters.AddWithValue("$idade", idade);
    command.Parameters.AddWithValue("$email", email);
    command.ExecuteNonQuery();
    Console.WriteLine("Cadastrado com sucesso!");
}

void Ler()
{
    Console.WriteLine("\n--- LISTA DE CONSUMIDORES ---");
    using var conexao = new SqliteConnection(connectionString);
    conexao.Open();
    var command = new SqliteCommand("SELECT * FROM Consumidores", conexao);
    using var leitor = command.ExecuteReader();
    while (leitor.Read())
    {
        Console.WriteLine($"ID: {leitor.GetInt32(0)} | Nome: {leitor.GetString(1)} | Idade: {leitor.GetInt32(2)} | Email: {leitor.GetString(3)}");
    }
}

void Atualizar()
{
    Console.Write("Digite o ID para atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    Console.WriteLine("1-Nome | 2-Idade | 3-Email | 4-Todos");
    string escolha = Console.ReadLine();

    using var conexao = new SqliteConnection(connectionString);
    conexao.Open();
    var command = conexao.CreateCommand();
    string sql = "";

    if (escolha == "1") {
        Console.Write("Novo Nome: ");
        command.Parameters.AddWithValue("$val", Console.ReadLine());
        sql = "UPDATE Consumidores SET Nome = $val WHERE Id = $id";
    }
    else if (escolha == "2") {
        Console.Write("Nova Idade: ");
        command.Parameters.AddWithValue("$val", int.Parse(Console.ReadLine()));
        sql = "UPDATE Consumidores SET Idade = $val WHERE Id = $id";
    }
    else if (escolha == "3") {
        Console.Write("Novo Email: ");
        command.Parameters.AddWithValue("$val", Console.ReadLine());
        sql = "UPDATE Consumidores SET Email = $val WHERE Id = $id";
    }
    else if (escolha == "4") {
        Console.Write("Novo Nome: "); string n = Console.ReadLine();
        Console.Write("Nova Idade: "); int i = int.Parse(Console.ReadLine());
        Console.Write("Novo Email: "); string e = Console.ReadLine();
        sql = "UPDATE Consumidores SET Nome = $n, Idade = $i, Email = $e WHERE Id = $id";
        command.Parameters.AddWithValue("$n", n);
        command.Parameters.AddWithValue("$i", i);
        command.Parameters.AddWithValue("$e", e);
    }

    command.CommandText = sql;
    command.Parameters.AddWithValue("$id", id);
    int rows = command.ExecuteNonQuery();
    Console.WriteLine(rows > 0 ? "Atualizado!" : "ID não encontrado.");
}

void Remover()
{
    Console.Write("Digite o ID do consumidor que deseja remover: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    using var conexao = new SqliteConnection(connectionString);
    conexao.Open();
    var command = conexao.CreateCommand();
    command.CommandText = "DELETE FROM Consumidores WHERE Id = $id";
    command.Parameters.AddWithValue("$id", id);

    int rows = command.ExecuteNonQuery();
    Console.WriteLine(rows > 0 ? "Removido com sucesso!" : "ID não encontrado.");
}