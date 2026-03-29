namespace PersonApi.Models;

public class PersonResponse
{
    private double _id;
    private string _firstname;
    private string _lastname;
    private decimal _yearOfBirth;

    public double Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Firstname
    {
        get { return _firstname; }
        set { _firstname = value; }
    }

    public string Lastname
    {
        get { return _lastname; }
        set { _lastname = value; }
    }

    public decimal YearOfBirth
    {
        get { return _yearOfBirth; }
        set
        {
            if (value > DateTime.Now.Year)
            {
                throw new System.Exception("Customer can not be born after current year");
            }
            _yearOfBirth = value;
        }
    }

    public PersonResponse() { }
    public PersonResponse(double id, string firstname, string lastname, decimal yearOfBirth)
    {
        if(yearOfBirth > DateTime.Now.Year)
        {
            throw new System.Exception("Customer can not be born after current year");
        }

        _id = id;
        _firstname = firstname;
        _lastname = lastname;
        _yearOfBirth = yearOfBirth;
    }
}

public class ObjProduct
{
    private double _id;
    private string _name;
    private string _type;
    private double _price;

    public double Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public double Price
    {
        get { return _price; }
        set { _price = value; }
    }

    public ObjProduct() { }
    public ObjProduct(double id, string name, string type)
    {
        _id = id;
        _name = name;
        _type = type;
    }
}

public class ObjPurchase
{
    private double _id;
    private double _customerId;
    private List<double> _productId;

    public double Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public double CustomerId
    {
        get { return _customerId; }
        set { _customerId = value; }
    }

    public List<double> ProductId
    {
        get { return _productId; }
        set { _productId = value; }
    }

    public ObjPurchase() { }

    public ObjPurchase(double id, double customerId, List<double> productId)
    {
        _id = id;
        _customerId = customerId;
        _productId = productId;
    }
}
