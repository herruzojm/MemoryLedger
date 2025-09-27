namespace MemoryLedgerApp.Utilities;

public static class InputHelper
{
    public static string Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? string.Empty;
    }

    public static string PromptRequired(string message)
    {
        while (true)
        {
            var value = Prompt(message);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            Console.WriteLine("El valor no puede estar vacío.");
        }
    }

    public static string? PromptOptionalString(string message)
    {
        var value = Prompt(message);
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static int PromptInt(string message, int? min = null, int? max = null)
    {
        while (true)
        {
            var input = Prompt(message);
            if (int.TryParse(input, out var value))
            {
                if (min.HasValue && value < min.Value)
                {
                    Console.WriteLine($"El valor debe ser mayor o igual a {min}.");
                    continue;
                }

                if (max.HasValue && value > max.Value)
                {
                    Console.WriteLine($"El valor debe ser menor o igual a {max}.");
                    continue;
                }

                return value;
            }

            Console.WriteLine("Introduce un número válido.");
        }
    }

    public static DateTime PromptDate(string message)
    {
        while (true)
        {
            var input = Prompt(message);
            if (DateTime.TryParse(input, out var date))
            {
                return date;
            }

            Console.WriteLine("Introduce una fecha válida (por ejemplo, 2024-03-21).");
        }
    }

    public static DateTime? PromptOptionalDate(string message)
    {
        var input = Prompt(message);
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (DateTime.TryParse(input, out var date))
        {
            return date;
        }

        Console.WriteLine("Fecha inválida, se ignorará el filtro de fecha.");
        return null;
    }

    public static int? PromptOptionalInt(string message)
    {
        var input = Prompt(message);
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (int.TryParse(input, out var value))
        {
            return value;
        }

        Console.WriteLine("Valor inválido, se ignorará el filtro.");
        return null;
    }
}
