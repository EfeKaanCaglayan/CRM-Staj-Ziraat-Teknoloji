namespace ZTCRM.Models;

public class Customer
{
    public int CustomerId {get;set;}
    public string CustomerType {get;set;}=string.Empty;
    public string? NationalId {get;set;} 
    public string? TaxNumber {get;set;} 
    public string NotifyChannel {get;set;} = string.Empty;
    public string Phone {get;set;} = string.Empty;
    public string Email {get;set;} = string.Empty;
    public string Address {get;set;} = string.Empty;
    public string FullName {get;set;} = string.Empty;
    public DateTime? BirthDate {get;set;}
    public DateTime? CreatedAt {get;set;}
    
    public bool IsActive {get;set;} 
}