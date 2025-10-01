public class Moto
{
    public int Id { get; set; }

    /// A placa da moto, preferencialmente no formato Mercosul.
    /// exemplo: BRA2E19
    public string Placa { get; set; } = null!;

    /// O nome do modelo da moto.
    /// exemplo: Honda Biz
    public string Modelo { get; set; } = null!;

    public int Ano { get; set; }
}