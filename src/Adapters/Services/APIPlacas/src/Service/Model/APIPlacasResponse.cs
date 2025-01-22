
using System.Text.Json.Serialization;

namespace Optimus.Services.Vehicles.APIPlacas.Service.Model
{
    public class DadosFipe
    {
        public string ano_modelo { get; set; }
        public string codigo_fipe { get; set; }
        public int codigo_marca { get; set; }
        public string codigo_modelo { get; set; }
        public string combustivel { get; set; }
        public int id_valor { get; set; }
        public string mes_referencia { get; set; }
        public int referencia_fipe { get; set; }
        public int score { get; set; }
        public string sigla_combustivel { get; set; }
        public string texto_marca { get; set; }
        public string texto_modelo { get; set; }
        public string texto_valor { get; set; }
        public int tipo_modelo { get; set; }
    }

    public class Extra
    {
        public string ano_fabricacao { get; set; }
        public string ano_modelo { get; set; }
        public string caixa_cambio { get; set; }
        public string cap_maxima_tracao { get; set; }
        public string carroceria { get; set; }
        public string chassi { get; set; }
        public string cilindradas { get; set; }
        public string combustivel { get; set; }
        public string di { get; set; }
        public string eixo_traseiro_dif { get; set; }
        public string eixos { get; set; }
        public string especie { get; set; }
        public string faturado { get; set; }
        public string grupo { get; set; }
        public string limite_restricao_trib { get; set; }
        public string linha { get; set; }
        public string media_preco { get; set; }
        public string modelo { get; set; }
        public string motor { get; set; }
        public string municipio { get; set; }
        public string nacionalidade { get; set; }
        public string peso_bruto_total { get; set; }
        public string placa { get; set; }
        public string placa_modelo_antigo { get; set; }
        public string placa_modelo_novo { get; set; }
        public string quantidade_passageiro { get; set; }
        public string registro_di { get; set; }
        public string renavam { get; set; }
        public string restricao_1 { get; set; }
        public string restricao_2 { get; set; }
        public string restricao_3 { get; set; }
        public string restricao_4 { get; set; }

        [JsonPropertyName("s.especie")]
        public string sespecie { get; set; }
        public string segmento { get; set; }
        public string situacao_chassi { get; set; }
        public string situacao_veiculo { get; set; }
        public string sub_segmento { get; set; }
        public string terceiro_eixo { get; set; }
        public string tipo_carroceria { get; set; }
        public string tipo_doc_faturado { get; set; }
        public string tipo_doc_importadora { get; set; }
        public string tipo_doc_prop { get; set; }
        public string tipo_montagem { get; set; }
        public string tipo_veiculo { get; set; }
        public string uf { get; set; }
        public string uf_faturado { get; set; }
        public string uf_placa { get; set; }
        public string unidade_local_srf { get; set; }
    }

    public class Fipe
    {
        public List<DadosFipe> dados { get; set; }
    }

    public record ConsultaPlaca
    {
        public string _id { get; set; }
        public string MARCA { get; set; }
        public string MODELO { get; set; }
        public string SUBMODELO { get; set; }
        public string VERSAO { get; set; }
        public string ano { get; set; }
        public string anoModelo { get; set; }
        public string chassi { get; set; }
        public string chassis { get; set; }
        public string city { get; set; }
        public string codigoRetorno { get; set; }
        public string codigoSituacao { get; set; }
        public string color { get; set; }
        public string cor { get; set; }
        public string data { get; set; }
        public string dataAtualizacaoAlarme { get; set; }
        public string dataAtualizacaoCaracteristicasVeiculo { get; set; }
        public string dataAtualizacaoRouboFurto { get; set; }
        public string datacache { get; set; }
        public DateTime date { get; set; }
        public Extra extra { get; set; }
        public int fabricationYear { get; set; }
        public Fipe fipe { get; set; }
        public bool hasUserLink { get; set; }
        public string infoProvider { get; set; }
        public List<string> listamodelo { get; set; }
        public string logo { get; set; }
        public string marca { get; set; }
        public string marcaModelo { get; set; }
        public string mensagemRetorno { get; set; }
        public string model { get; set; }
        public string modelo { get; set; }
        public string municipio { get; set; }
        public string origem { get; set; }
        public string placa { get; set; }
        public string placa_alternativa { get; set; }
        public string plate { get; set; }
        public string segmento { get; set; }
        public string situacao { get; set; }
        public string state { get; set; }
        public string sub_segmento { get; set; }
        public string uf { get; set; }
        public int year { get; set; }
    }

    public record VehiclePlateDownloaded : ConsultaPlaca, IDomainEvent;

}
