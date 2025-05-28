/*Телефонная книга. Должна быть реализована CRUD функциональность:
Должен уметь принимать от пользователя номер и имя телефона.
Сохранять номер в файле phonebook.txt. (При завершении программы либо при добавлении).
Вычитывать из файла сохранённые номера. (При старте программы).
Удалять номера.
Получать абонента по номеру телефона.
Получать номер телефона по имени абонента.
Обращение к Phonebook должно быть как к классу-одиночке.
Внутри должна быть коллекция с абонентами.
Для обращения с абонентами нужно завести класс Abonent. С полями «номер телефона», «имя».
Не дать заносить уже записанного абонента.*/

using System;
using System.Collections.Generic;
using System.IO;


public class Abonent
{
    public string Number { get; set; }
    public string Name { get; set; }

    public Abonent(string number, string name)
    {
        Number = number;
        Name = name;
    }
}



public sealed class PhoneBook  
{
    private static readonly Lazy<PhoneBook> lazy = new Lazy<PhoneBook>(() => new PhoneBook());

    public static PhoneBook Instance { get { return lazy.Value; } }

    private Dictionary<string, string> _abonents = new Dictionary<string, string>();

    private const string filePath = "phonebook.txt";

    private PhoneBook()
    {
        LoadFromFile();
    }
      
    public void AddAbonent(Abonent abonent)  
    {
        if (_abonents.ContainsKey(abonent.Number))
            throw new Exception("Абонент с таким номером уже существует.");

        _abonents.Add(abonent.Number, abonent.Name);
        SaveToFile(); 
    }
    
    public bool RemoveAbonentByNumber(string number)  
    {
        if (!_abonents.ContainsKey(number)) return false;
        _abonents.Remove(number);
        SaveToFile(); 
        return true;
    }

    public string GetNameByNumber(string number)  
    {
        if (_abonents.TryGetValue(number, out var name))
            return name;
        else
            return null;
    }

    public List<string> GetNumbersByName(string name)  
    {
        var numbers = new List<string>();
        foreach (var entry in _abonents)
        {
            if (entry.Value.Equals(name))
                numbers.Add(entry.Key);
        }
        return numbers;
    }

    private void LoadFromFile()  
    {
        try
        {
            if (!File.Exists(filePath)) return;
            using (StreamReader reader = File.OpenText(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (line != "")
                    {
                        var parts = line.Split(';');
                        if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]) &&
                           !string.IsNullOrEmpty(parts[1]))
                            _abonents.Add(parts[0], parts[1]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки из файла: {ex.Message}");
        }
    }

    private void SaveToFile() 
    {
        try
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                foreach (KeyValuePair<string, string> entry in _abonents)
                {
                    writer.WriteLine($"{entry.Key};{entry.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения в файл: {ex.Message}");
        }
    }
}



class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nТелефонная книга");
            Console.WriteLine("1. Добавить абонента");
            Console.WriteLine("2. Найти абонента по номеру");
            Console.WriteLine("3. Найти абонента по имени");
            Console.WriteLine("4. Удалить абонента");
            Console.WriteLine("5. Выход");
            Console.Write("Ваш выбор: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddContact();
                    break;
                case 2:
                    FindByNumber();
                    break;
                case 3:
                    FindByName();
                    break;
                case 4:
                    DeleteContact();
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный ввод! Попробуйте снова.");
                    break;
            }
        }
    }

    static void AddContact()
    {
        Console.Write("Введите номер телефона: ");
        string number = Console.ReadLine();
        Console.Write("Введите имя контакта: ");
        string name = Console.ReadLine();

        try
        {
            PhoneBook.Instance.AddAbonent(new Abonent(number, name));
            Console.WriteLine("Контакт успешно добавлен!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static void FindByNumber()
    {
        Console.Write("Введите номер телефона: ");
        string number = Console.ReadLine();
        string name = PhoneBook.Instance.GetNameByNumber(number);
        if (name != null)
            Console.WriteLine($"Имя абонента: {name}");
        else
            Console.WriteLine("Такой абонент не найден.");
    }

    static void FindByName()
    {
        Console.Write("Введите имя абонента: ");
        string name = Console.ReadLine();
        var numbers = PhoneBook.Instance.GetNumbersByName(name);
        if (numbers.Count > 0)
        {
            Console.WriteLine("Найденные номера:");
            foreach (var num in numbers)
                Console.WriteLine(num);
        }
        else
            Console.WriteLine("Таких абонентов не найдено.");
    }

    static void DeleteContact()
    {
        Console.Write("Введите номер телефона для удаления: ");
        string number = Console.ReadLine();
        if (PhoneBook.Instance.RemoveAbonentByNumber(number))
            Console.WriteLine("Контакт удалён успешно.");
        else
            Console.WriteLine("Такой абонент не найден.");
    }
}

