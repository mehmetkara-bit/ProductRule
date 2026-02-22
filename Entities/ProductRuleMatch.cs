public class ProductRuleMatch {
    public int ProductRuleMatchId { get; set; }
    public int RuleDefinitionId { get; set; }
    public int ProductId { get; set; }
    public DateTime MatchedAt { get; set; }

    public Product Product { get; set; }
    public RuleDefinition RuleDefinition { get; set; }
}
