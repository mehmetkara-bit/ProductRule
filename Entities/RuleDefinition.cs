public class RuleDefinition {
    public int RuleDefinitionId { get; set; }
    public string RuleName { get; set; }
    public string? Color { get; set; }
    public string? ShippingCountry { get; set; }
    public string? ProductionCountry { get; set; }

    public int ProductTestId { get; set; }
    public ProductTest ProductTest { get; set; } // Navigation property

}