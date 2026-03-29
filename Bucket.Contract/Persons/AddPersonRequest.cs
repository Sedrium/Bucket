namespace Bucket.Contract.Persons;

public record AddPersonRequest(double Id, string Firstname, string Lastname, DateOnly DateOfBirth);
