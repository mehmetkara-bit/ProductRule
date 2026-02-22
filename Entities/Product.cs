public class Product {
    public int ProductNo { get; set; } // Primary Key
    public string ProductionCountry { get; set; }
    public DateTime ProductionDate { get; set; }
    public ProductDetail Detail { get; set; }
}