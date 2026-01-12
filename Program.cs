namespace SingleResponsibleStudent
{
    // Клас для зберігання особистих даних студента
    public class Person
    {
        public required string FirstName { get; init; } = "";
        public required string LastName { get; init; } = "";
        public string? Patronymic { get; init; }
        public DateOnly BirthDate { get; init; }

        public string GetFullName() => 
            $"{LastName} {FirstName} {Patronymic}".Trim();
    }


    // Допоміжний клас для роботи зі знаком зодіаку
    public static class AstroProfile
    {
        // Єдина відповідальність: визначити знак зодіаку через сервіс
        public static string GetZnakZodiaka(DateOnly birthDate,
            IHoroscopeService? service)
        {
            return service?.ResolveZodiacSign(birthDate)
                ?? "Сервіс гороскопів недоступний";
        }
    }


    // Клас, що описує адресу
    public class Address
    {
        public string? Country { get; init; }
        public string? Region { get; init; }
        public string? City { get; init; }
        public string? Street { get; init; }
        public ushort? HouseNumber { get; init; }
        public char? Korpus { get; init; }
        public string? PostalCode { get; private set; }  // лише через метод

        // Повертає нову адресу з доданим/оновленим поштовим індексом
        public Address WithPostalCode(IPostalCodeService? service)
        {
            if (service is null) return this;

            var postalCode = service.ResolvePostalCode(this);
            return new Address
            {
                Country = Country,
                Region = Region,
                City = City,
                Street = Street,
                HouseNumber = HouseNumber,
                Korpus = Korpus,
                PostalCode = postalCode
            };
        }
    }


    // Інформація про навчання студента
    public class StudyInfo
    {
        public DateOnly StartDate { get; init; }
        public int Kurs { get; private set; }
        public string? GroupName { get; init; }
        public string? Specialization { get; init; }
        public int StudentsCount { get; init; }

        public void AdvanceCourse() => Kurs++;
    }


    // Відвідуваність занять та запізнення
    public class Attendance
    {
        public int LessonsVisited { get; set; }
        public int LessonsLate { get; set; }
    }


    // Успішність з домашніх завдань
    public class HomeworkPerformance
    {
        public int[]? DzRates { get; set; }
        public float DzAverageRate { get; private set; }

        public void RecalculateAverage()
        {
            DzAverageRate = DzRates is { Length: > 0 }
                ? (float)DzRates.Average()
                : 0;
        }
    }


    // Успішність з практичних занять
    public class PracticePerformance
    {
        public int[]? PracticeRates { get; set; }
        public float PracticeAverageRate { get; private set; }

        public void RecalculateAverage()
        {
            PracticeAverageRate = PracticeRates is { Length: > 0 }
                ? (float)PracticeRates.Average()
                : 0;
        }
    }


    // Успішність з іспитів
    public class ExamPerformance
    {
        public int[]? ExamRates { get; set; }
        public float ExamAverageRate { get; private set; }

        public void RecalculateAverage()
        {
            ExamAverageRate = ExamRates is { Length: > 0 }
                ? (float)ExamRates.Average()
                : 0;
        }
    }


    // Успішність із заліків
    public class ZalikPerformance
    {
        public int[]? ZalikRates { get; set; }
        public float ZalikAverageRate { get; private set; }

        public void RecalculateAverage()
        {
            ZalikAverageRate = ZalikRates is { Length: > 0 }
                ? (float)ZalikRates.Average()
                : 0;
        }
    }


    // Успішність з одного конкретного предмета
    public class SubjectPerformance
    {
        public required string SubjectName { get; init; } = "";
        public required string TeacherName { get; init; } = "";

        public HomeworkPerformance Homework { get; } = new();
        public PracticePerformance Practice { get; } = new();
        public ExamPerformance Exam { get; } = new();
        public ZalikPerformance Zalik { get; } = new();
    }


    // Головний агрегат - студент
    public class StudentAggregate
    {
        public Person Person { get; private set; }
        public Address Address { get; private set; }
        public StudyInfo StudyInfo { get; private set; }
        public Attendance Attendance { get; private set; }
        private readonly List<SubjectPerformance> _subjects = [];

        // Список усіх предметів (тільки для читання)
        public IReadOnlyCollection<SubjectPerformance> Subjects =>
            _subjects.AsReadOnly();

        public StudentAggregate(Person person, Address address,
            StudyInfo studyInfo)
        {
            Person = person ??
                throw new ArgumentNullException(nameof(person));
            Address = address ??
                throw new ArgumentNullException(nameof(address));
            StudyInfo = studyInfo ??
                throw new ArgumentNullException(nameof(studyInfo));
            Attendance = new Attendance();
        }

        // Додає або оновлює інформацію про предмет
        public void AddOrUpdateSubject(SubjectPerformance subject)
        {
            var existing = _subjects.FirstOrDefault(s =>
            s.SubjectName == subject.SubjectName);
            if (existing != null) _subjects.Remove(existing);
            _subjects.Add(subject);
        }

        //public double? CalculateTotalAverage(IGradeCalculator calculator)
        //    => calculator?.CalculateTotalAverage(this);
    }


    // Інтерфейс для отримання знаку зодіаку
    public interface IHoroscopeService
    {
        string ResolveZodiacSign(DateOnly birthDate);
    }


    // Інтерфейс для визначення поштового індексу
    public interface IPostalCodeService
    {
        string? ResolvePostalCode(Address address);
    }
}


//// використовуючи принцип SRP, розбити клас Student на 5+ дрібніших типів 
///(не забуваючи про зв'язки між класами)

//class Student
//{
//    public string? FirstName { get; set; }
//    public string? Surname { get; set; }
//    public string? Lastname { get; set; }
//    public string? Country { get; set; }
//    public string? Region { get; set; }
//    public string? City { get; set; }
//    public string? Street { get; set; }
//    public int HouseNumber { get; set; }
//    public char Korpus { get; set; }
//    public short PostalCode { get; set; }
//    public int BirthDay { get; set; }
//    public int BirthMonth { get; set; }
//    public int BirthYear { get; set; }
//    public string? ZnakZodiaka { get; set; }
//    public int StartDay { get; set; }
//    public int StartMonth { get; set; }
//    public int StartYear { get; set; }
//    public int Kurs { get; set; }
//    public string? GroupName { get; set; }
//    public string? Specialization { get; set; }
//    public int StudentsCount { get; set; }
//    public int LessonsVisited { get; set; }
//    public int LessonsLate { get; set; }
//    public string? TeacherName { get; set; }
//    public string? SubjectName { get; set; }
//    public int[]? DzRates { get; set; }
//    public float DzAverageRate { get; set; }
//    public int[]? PracticeRates { get; set; }
//    public float PracticeAverageRate { get; set; }
//    public int[]? ExamRates { get; set; }
//    public float ExamAverageRate { get; set; }
//    public int[]? ZachetRates { get; set; }
//    public int ZachetCount { get; set; }
//    public float ZachetAverageRate { get; set; }
//    public double TotalAverageRate { get; set; }
//}