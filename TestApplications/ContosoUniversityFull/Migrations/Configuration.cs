using ContosoUniversityFull.DAL;
using ContosoUniversityFull.Models;

namespace ContosoUniversityFull.Migrations
{
    using ContosoUniversityFull.Models;
    using ContosoUniversityFull.DAL;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SchoolContext>
    {
        private readonly Random random = new Random(DateTime.Now.Millisecond);
        private readonly List<string> firstNames = new List<string> { "Abigail ", "Alexandra ", "Alison ", "Amanda ", "Amelia ", "Amy ", "Andrea ", "Angela ", "Anna ", "Anne ", "Audrey ", "Ava ", "Bella ", "Bernadette ", "Carol ", "Caroline ", "Carolyn ", "Chloe ", "Claire ", "Deirdre ", "Diana ", "Diane ", "Donna ", "Dorothy ", "Elizabeth ", "Ella ", "Emily ", "Emma ", "Faith ", "Felicity ", "Fiona ", "Gabrielle ", "Grace ", "Hannah ", "Heather ", "Irene ", "Jan ", "Jane ", "Jasmine ", "Jennifer ", "Jessica ", "Joan ", "Joanne ", "Julia ", "Karen ", "Katherine ", "Kimberly ", "Kylie ", "Lauren ", "Leah ", "Lillian ", "Lily ", "Lisa ", "Madeleine ", "Maria ", "Mary ", "Megan ", "Melanie ", "Michelle ", "Molly ", "Natalie ", "Nicola ", "Olivia ", "Penelope ", "Pippa ", "Rachel ", "Rebecca ", "Rose ", "Ruth ", "Sally ", "Samantha ", "Sarah ", "Sonia ", "Sophie ", "Stephanie ", "Sue ", "Theresa ", "Tracey ", "Una ", "Vanessa ", "Victoria ", "Virginia ", "Wanda ", "Wendy ", "Yvonne ", "Zoe ", "Adam ", "Adrian ", "Alan ", "Alexander ", "Andrew ", "Anthony ", "Austin ", "Benjamin ", "Blake ", "Boris ", "Brandon ", "Brian ", "Cameron ", "Carl ", "Charles ", "Christian ", "Christopher ", "Colin ", "Connor ", "Dan ", "David ", "Dominic ", "Dylan ", "Edward ", "Eric ", "Evan ", "Frank ", "Gavin ", "Gordon ", "Harry ", "Ian ", "Isaac ", "Jack ", "Jacob ", "Jake ", "James ", "Jason ", "Joe ", "John ", "Jonathan ", "Joseph ", "Joshua ", "Julian ", "Justin ", "Keith ", "Kevin ", "Leonard ", "Liam ", "Lucas ", "Luke ", "Matt ", "Max ", "Michael ", "Nathan ", "Neil ", "Nicholas ", "Oliver ", "Owen ", "Paul ", "Peter ", "Phil ", "Piers ", "Richard ", "Robert ", "Ryan ", "Sam ", "Sean ", "Sebastian ", "Simon ", "Stephen ", "Steven ", "Stewart ", "Thomas ", "Tim ", "Trevor ", "Victor ", "Warren ", "William" };
        private readonly List<string> lastNames = new List<string> { "Abraham ", "Allan ", "Alsop ", "Anderson ", "Arnold ", "Avery ", "Bailey ", "Baker ", "Ball ", "Bell ", "Berry ", "Black ", "Blake ", "Bond ", "Bower ", "Brown ", "Buckland ", "Burgess ", "Butler ", "Cameron ", "Campbell ", "Carr ", "Chapman ", "Churchill ", "Clark ", "Clarkson ", "Coleman ", "Cornish ", "Davidson ", "Davies ", "Dickens ", "Dowd ", "Duncan ", "Dyer ", "Edmunds ", "Ellison ", "Ferguson ", "Fisher ", "Forsyth ", "Fraser ", "Gibson ", "Gill ", "Glover ", "Graham ", "Grant ", "Gray ", "Greene ", "Hamilton ", "Hardacre ", "Harris ", "Hart ", "Hemmings ", "Henderson ", "Hill ", "Hodges ", "Howard ", "Hudson ", "Hughes ", "Hunter ", "Ince ", "Jackson ", "James ", "Johnston ", "Jones ", "Kelly ", "Kerr ", "King ", "Knox ", "Lambert ", "Langdon ", "Lawrence ", "Lee ", "Lewis ", "Lyman ", "MacDonald ", "Mackay ", "Mackenzie ", "MacLeod ", "Manning ", "Marshall ", "Martin ", "Mathis ", "May ", "McDonald ", "McLean ", "McGrath ", "Metcalfe ", "Miller ", "Mills ", "Mitchell ", "Morgan ", "Morrison ", "Murray ", "Nash ", "Newman ", "Nolan ", "North ", "Ogden ", "Oliver ", "Paige ", "Parr ", "Parsons ", "Paterson ", "Payne ", "Peake ", "Peters ", "Piper ", "Poole ", "Powell ", "Pullman ", "Quinn ", "Rampling ", "Randall ", "Rees ", "Reid ", "Roberts ", "Robertson ", "Ross ", "Russell ", "Rutherford ", "Sanderson ", "Scott ", "Sharp ", "Short ", "Simpson ", "Skinner ", "Slater ", "Smith ", "Springer ", "Stewart ", "Sutherland ", "Taylor ", "Terry ", "Thomson ", "Tucker ", "Turner ", "Underwood ", "Vance ", "Vaughan ", "Walker ", "Wallace ", "Walsh ", "Watson ", "Welch ", "White ", "Wilkins ", "Wilson ", "Wright ", "Young" };

        private int firstNamesMax;
        private int lastNamesMax;

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SchoolContext context)
        {
            firstNamesMax = firstNames.Count;
            lastNamesMax = lastNames.Count;

            // Look for any students.
            if (context.Students.Any())
            {
                // DB has been seeded
                return;
            }

            var students = new List<Student>
            {
                new Student { FirstMidName = "Carson",   LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Student { FirstMidName = "Meredith", LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Arturo",   LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Gytis",    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Yan",      LastName = "Li",
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { FirstMidName = "Peggy",    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Student { FirstMidName = "Laura",    LastName = "Norman",
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { FirstMidName = "Nino",     LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01") },
                new Student{ FirstMidName="Christoph",   LastName="Hauer",
                    EnrollmentDate =DateTime.Parse("2005-09-01")},
                new Student{FirstMidName="Max",          LastName="Mustermann",
                    EnrollmentDate =DateTime.Parse("2005-09-01")}

            };

            DateTime createDate = DateTime.Now;

            for (int i = 0; i < 400; i++)
            {
                students.Add(new Student()
                {
                    FirstMidName = firstNames[random.Next(0, firstNamesMax)],
                    LastName = lastNames[random.Next(0, lastNamesMax)],
                    EnrollmentDate = createDate
                });
            }

            students.ForEach(s => context.Students.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var instructors = new List<Instructor>
            {
                new Instructor { FirstMidName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Instructor { FirstMidName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Instructor { FirstMidName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Instructor { FirstMidName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Instructor { FirstMidName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12") }
            };
            instructors.ForEach(s => context.Instructors.AddOrUpdate(p => p.LastName, s));
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Abercrombie").ID },
                new Department { Name = "Mathematics", Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Fakhouri").ID },
                new Department { Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
                new Department { Name = "Economics",   Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Kapoor").ID },
                new Department { Name = "Software Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID  = instructors.Single( i => i.LastName == "Harui").ID },
            };
            departments.ForEach(s => context.Departments.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();

            var courses = new List<Course>
            {
                new Course {CourseID = 1050, Title = "Chemistry",      Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 4022, Title = "Microeconomics", Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 4041, Title = "Macroeconomics", Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 1045, Title = "Calculus",       Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 3141, Title = "Trigonometry",   Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 2021, Title = "Composition",    Credits = 3,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Instructors = new List<Instructor>()
                },
                new Course {CourseID = 2042, Title = "Literature",     Credits = 4,
                  DepartmentID = departments.Single( s => s.Name == "English").DepartmentID,
                  Instructors = new List<Instructor>()
                },
            };

            int startNumber = 6001;
            List<string> createCourses = new List<string> {"Functional Programming","Algorithmics", "Machine Learning", "Software Security", "Digital Forensics",
                "Database Systems", "Knowledge Management", "Software Testing","Project Management", "Network Engineering", "Distributed Systems"};

            foreach (string newCourse in createCourses)
            {
                courses.Add(new Course
                {
                    CourseID = startNumber,
                    Title = newCourse,
                    Credits = random.Next(1, 8),
                    DepartmentID = departments.Single(s => s.Name == "Software Engineering").DepartmentID
                });
                startNumber++;
            }

            courses.ForEach(s => context.Courses.AddOrUpdate(p => p.CourseID, s));
            context.SaveChanges();

            var officeAssignments = new List<OfficeAssignment>
            {
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Fakhouri").ID,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Harui").ID,
                    Location = "Gowan 27" },
                new OfficeAssignment {
                    InstructorID = instructors.Single( i => i.LastName == "Kapoor").ID,
                    Location = "Thompson 304" },
            };
            officeAssignments.ForEach(s => context.OfficeAssignments.AddOrUpdate(p => p.InstructorID, s));
            context.SaveChanges();

            AddOrUpdateInstructor(context, "Chemistry", "Kapoor");
            AddOrUpdateInstructor(context, "Chemistry", "Harui");
            AddOrUpdateInstructor(context, "Microeconomics", "Zheng");
            AddOrUpdateInstructor(context, "Macroeconomics", "Zheng");

            AddOrUpdateInstructor(context, "Calculus", "Fakhouri");
            AddOrUpdateInstructor(context, "Trigonometry", "Harui");
            AddOrUpdateInstructor(context, "Composition", "Abercrombie");
            AddOrUpdateInstructor(context, "Literature", "Abercrombie");

            context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    Grade = Grade.A
                },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    Grade = Grade.C
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alexander").ID,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                     StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                     StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Alonso").ID,
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Anand").ID,
                    CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                    Grade = Grade.B
                 },
                new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Barzdukas").ID,
                    CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Li").ID,
                    CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                    Grade = Grade.B
                 },
                 new Enrollment {
                    StudentID = students.Single(s => s.LastName == "Justice").ID,
                    CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                    Grade = Grade.B
                 }
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                         s.Student.ID == e.StudentID &&
                         s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            context.SaveChanges();
        }

        void AddOrUpdateInstructor(SchoolContext context, string courseTitle, string instructorName)
        {
            var crs = context.Courses.SingleOrDefault(c => c.Title == courseTitle);
            var inst = crs.Instructors.SingleOrDefault(i => i.LastName == instructorName);
            if (inst == null)
                crs.Instructors.Add(context.Instructors.Single(i => i.LastName == instructorName));
        }
    }
}
