namespace Backend.Api.Services;

public sealed class AddNumbersService
{
    public int Add(int firstNumber, int secondNumber)
    {
        return firstNumber + secondNumber;
    }
}