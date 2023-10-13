using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeuProjeto.Controllers
{
    [ApiController]
    [Route("tarefa")]
    public class SalvarTarefaController : ControllerBase
    {
        private const string FilePath = "data.json";

        [HttpPost]
        public IActionResult Post([FromBody] RequestModel request)
        {
            // Validar se o modelo é válido (nome e email não estão vazios)
            if (string.IsNullOrEmpty(request.nomeTarefa) || request.dataTarefa == default)
            {
                return BadRequest("Nome e data devem ser informados.");
            }

            DateTime dataAtual = DateTime.Now;

            if (request.dataTarefa < dataAtual)
            {
                return BadRequest("A data informada é anterior à data atual!");
            }

            // Ler o conteúdo atual do arquivo (se existir)
            List<Tarefa> existingData = new List<Tarefa>();
            if (System.IO.File.Exists(FilePath))
            {
                string jsonString = System.IO.File.ReadAllText(FilePath);
                existingData = JsonSerializer.Deserialize<List<Tarefa>>(jsonString);
            }

            Tarefa tarefa = new Tarefa();

            tarefa.id = existingData.Count + 1;
            tarefa.nomeTarefa = request.nomeTarefa;
            tarefa.dataTarefa = request.dataTarefa;

            // Adicionar o novo objeto à lista
            existingData.Add(tarefa);

            // Serializar a lista completa para JSON
            string updatedJson = JsonSerializer.Serialize(existingData);

            // Salvar o JSON atualizado no arquivo
            System.IO.File.WriteAllText(FilePath, updatedJson);

            return new CreatedResult("/tarefas", "Dados salvos com sucesso.");
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Ler o conteúdo atual do arquivo (se existir)
            List<Tarefa> existingData = new List<Tarefa>();
            if (System.IO.File.Exists(FilePath))
            {
                string jsonString = System.IO.File.ReadAllText(FilePath);
                existingData = JsonSerializer.Deserialize<List<Tarefa>>(jsonString);
            }

            // Ordenar as tarefas em ordem cronológica
            existingData.Sort((t1, t2) => t1.dataTarefa.CompareTo(t2.dataTarefa));

            // Dicionário para agrupar as tarefas por data
            var tarefasAgrupadas = new Dictionary<DateTime, List<Tarefa>>();

            // Itera sobre as tarefas e as agrupa por data
            foreach (var tarefa in existingData)
            {
                if (tarefasAgrupadas.ContainsKey(tarefa.dataTarefa))
                {
                    tarefasAgrupadas[tarefa.dataTarefa].Add(tarefa);
                }
                else
                {
                    tarefasAgrupadas[tarefa.dataTarefa] = new List<Tarefa> { tarefa };
                }
            }

            // Cria um objeto anônimo com os grupos de tarefas
            var gruposDeTarefas = new List<object>();

            foreach (var grupo in tarefasAgrupadas)
            {
                var grupoDeTarefas = new
                {
                    data = grupo.Key,
                    tarefas = grupo.Value
                };

                gruposDeTarefas.Add(grupoDeTarefas);
            }

            // Converte o objeto em JSON
            string json = JsonSerializer.Serialize(gruposDeTarefas, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Retorna 200 e os dados que estão salvos
            return Ok(json);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Ler o conteúdo atual do arquivo (se existir)
            List<Tarefa> existingData = new List<Tarefa>();
            if (System.IO.File.Exists(FilePath))
            {
                string jsonString = System.IO.File.ReadAllText(FilePath);
                existingData = JsonSerializer.Deserialize<List<Tarefa>>(jsonString);
            }

            // Procurar a tarefa pelo ID
            Tarefa tarefa = existingData.FirstOrDefault(t => t.id == id);
            if (tarefa == null)
            {
                return NotFound("Tarefa não encontrada.");
            }

            // Remover a tarefa da lista
            existingData.Remove(tarefa);

            // Serializar a lista atualizada para JSON
            string updatedJson = JsonSerializer.Serialize(existingData);

            // Salvar o JSON atualizado no arquivo
            System.IO.File.WriteAllText(FilePath, updatedJson);

            return Ok("Tarefa excluída com sucesso.");
        }
    }

    public class RequestModel
    {
        [JsonPropertyName("nome_tarefa")]
        public string nomeTarefa { get; set; }

        [JsonPropertyName("data_tarefa")]
        public DateTime dataTarefa { get; set; }
    }

    public class Tarefa
    {
        public int id { get; set; }
        public string nomeTarefa { get; set; }
        public DateTime dataTarefa { get; set; }
    }
}