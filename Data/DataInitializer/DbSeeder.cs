using LifeLongApi.Dtos;
using LifeLongApi.Models;
using LifeLongApi.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifeLongApi.Data.DataInitializer
{
    public class DbSeeder
    {
        private static IUserService _userService;
        private static IRoleService _roleService;
        private static ICategoryService _categoryService;
        private static IFollowService _followService;
        private static ITopicService _topicService;

        public DbSeeder(IUserService userService,
                        IRoleService roleService,
                        ICategoryService categoryService,
                        IFollowService followService,
                        ITopicService topicService)
        {
            _categoryService = categoryService;
            _followService = followService;
            _topicService = topicService;
            _userService = userService;
            _roleService = roleService;
        }
        public async  Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCategoriesAsync();
            await SeedFieldOfInterestsAsync();
            await SeedFieldOfInterestsForDefaultUsersAsync();
            await SeedMentorshipRequestAsync();

        }

        private static async Task SeedUsersAsync()
        {
            var users = new List<RegisterDto>() {
                new RegisterDto {
                Email = "admin@lifelong.com",
                FirstName = "Admin",
                LastName = "Account",
                Password = "Lifelong1$",
                PhoneNumber = "08036566809",
                Gender = "Male",
                Role = "Admin"
                },
                new RegisterDto {
                Email = "mentor@lifelong.com",
                FirstName = "Lifelong",
                LastName = "Mentor",
                Password = "Mentor1$",
                PhoneNumber = "08055165372",
                Gender = "Female",
                Role = "Mentor"
                },
                new RegisterDto {
                Email = "mentee@lifelong.com",
                FirstName = "Lifelong",
                LastName = "Mentee",
                Password = "Mentee1$",
                PhoneNumber = "08140715723",
                Gender = "Female",
                Role = "Mentee"
                }
            };

            try
            {
                foreach (var user in users)
                {
                    var result = await _userService.CreateUserAsync(user);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            

        }

        private static async Task SeedRolesAsync()
        {
            var roleNames = new List<string>{
                "Admin", "Mentor", "Mentee"
            };

            foreach (var roleName in roleNames)
            {
                var result = await _roleService.NewRoleAsync(roleName);
                if (!result.Success)
                {
                    //error
                    //log error here..
                }
            }
        }

        private static async Task SeedCategoriesAsync()
        {
            var categoryNames = new List<string>{
               "Development",
                "Business",
                "Finance & Accounting",
                "IT & Software",
                "Office Productivity",
                "Personal Development",
                "Design",
                "Marketing",
                "Lifestyle",
                "Photography",
                "Health & Fitness",
                "Music",
                "Teaching & Academics",
                "Others"
            };

            foreach (var categoryName in categoryNames)
            {
                await _categoryService.AddCategoryAsync(categoryName);
            }
        }

        private static async Task SeedFieldOfInterestsAsync()
        {
            var devTopics = new Dictionary<string, List<string>>{
                {"Development", new List<string>{
                                                        "Python",
                                                        "Data Science",
                                                        "Web Development",
                                                        "JavaScript",
                                                        "Machine Learning",
                                                        "Java",
                                                        "React",
                                                        "C#",
                                                        "Unity",
                                                        "Data Analysis",
                                                        "Angular",
                                                        "Android Development",
                                                        "CSS",
                                                        "Deep Learning",
                                                        "iOS Development",
                                                        "C++",
                                                        "Node.Js",
                                                        "Google Flutter",
                                                        "Swift",
                                                        "Game Development Fundamentals"
                                                    }
                },
                {"Finance & Accounting", new List<string>{
                                                                    "Stock Trading",
                                                                    "Financial Analysis",
                                                                    "Investing",
                                                                    "Technical Analysis",
                                                                    "Forex",
                                                                    "Financial Modeling",
                                                                    "Finance Fundamentals",
                                                                    "Excel",
                                                                    "Day Trading",
                                                                    "Accounting",
                                                                    "Options Trading",
                                                                    "Python",
                                                                    "Financial Trading",
                                                                    "Personal Finance",
                                                                    "Investment Banking",
                                                                    "Financial Accounting",
                                                                    "CFA",
                                                                    "Stock Options",
                                                                    "Cryptocurrency",
                                                                    "Algorithmic Trading",
                                                                }
                },
                {"IT & Software", new List<string>{
                                                            "AWS Certification",
                                                            "Ethical Hacking",
                                                            "AWS Certified Solutions Architect - Associate",
                                                            "Microsoft Certification",
                                                            "Cisco CCNA",
                                                            "Cyber Security",
                                                            "CompTIA A+",
                                                            "Linux",
                                                            "CCNA 200-301",
                                                            "AWS Certified Developer - Associate",
                                                            "CompTIA Security+",
                                                            "Kubernetes",
                                                            "AWS Certified Cloud Practitioner",
                                                            " CompTIA Network+",
                                                            "Microsoft Azure",
                                                            "Microsoft AZ-900",
                                                            "Oracle Certification",
                                                            "Network Security",
                                                            "AWS Certified Solutions Architect - Professional",
                                                            "Windows Server"
                                                        }
                },
                {"Design", new List<string>{
                                            "Photoshop",
                                            "Drawing",
                                            "Adobe Illustrator",
                                            "Graphic Design",
                                            "Blender",
                                            "Character Animation",
                                            "User Experience Design",
                                            "3D Modeling",
                                            "Web Design",
                                            "After Effects",
                                            "Digital Painting",
                                            "User Interface",
                                            "Adobe XD",
                                            "Interior Design",
                                            "Character Design",
                                            " WordPress",
                                            "AutoCAD",
                                            "Motion Graphics",
                                            "InDesign",
                                            "Procreate Digital Illustration App"
                                        }
                },
                {"Photography", new List<string>{
                                                    "Photography",
                                                    "Video Editing",
                                                    "Adobe Premiere",
                                                    "Adobe Lightroom",
                                                    "Video Production",
                                                    "Filmmaking",
                                                    "Photoshop",
                                                    "iPhone Photography",
                                                    "DSLR",
                                                    "Affinity Photo",
                                                    "DaVinci Resolve",
                                                    "Videography",
                                                    "Final Cut Pro",
                                                    "Color Grading",
                                                    "Photoshop Retouching",
                                                    "Digital Photography",
                                                    "Portrait Photography",
                                                    "Image Editing",
                                                   " Food Photography",
                                                    "Night Photography",
                                            }
                 },
                {"Teaching & Academics", new List<string>{
                                                                    "English Language",
                                                                    "Spanish Language",
                                                                    "German Language",
                                                                    "Japanese Language",
                                                                    "French Language",
                                                                    "IELTS",
                                                                    "English Grammar",
                                                                    "Math",
                                                                    "English Conversation",
                                                                    "Psychology",
                                                                    "Online Course Creation",
                                                                    "Statistics",
                                                                    "Calculus",
                                                                    "Sign Language",
                                                                    "Linear Algebra",
                                                                    "Data Structures",
                                                                    "Algorithms",
                                                                    "Probability",
                                                                    "Counseling",
                                                                    "Italian Language"
                                                                }
                 },
                {"Office Productivity", new List<string>{
                                                                "Excel",
                                                                "Excel VBA",
                                                                "Excel Formulas and Functions",
                                                                "Data Analysis",
                                                                "PowerPoint",
                                                                "Pivot Tables",
                                                                "Microsoft Word",
                                                                "Microsoft Power BI",
                                                                "Microsoft Access",
                                                                "Excel Dashboard",
                                                                "Microsoft Project",
                                                                "Excel Macros",
                                                                "Microsoft Office",
                                                                "Excel Shortcut",
                                                                "Power Pivot",
                                                                "Microsoft Office 365",
                                                                "Data Visualization",
                                                                "Data Modeling",
                                                                "SAP",
                                                                "Microsoft Teams"
                                                            }
                 },
                {"Marketing", new List<string>{
                                                   " Digital Marketing",
                                                    "Instagram Marketing",
                                                    "Social Media Marketing",
                                                    "SEO",
                                                    "Facebook Ads",
                                                    "Facebook Marketing",
                                                    "Google Ads (Adwords)",
                                                    "Copywriting",
                                                    "Marketing Strategy",
                                                    "Google Ads (AdWords) Certification",
                                                    "YouTube Marketing",
                                                    "PPC Advertising",
                                                    "Google Analytics",
                                                    "YouTube Audience Growth",
                                                    "WordPress",
                                                    "Google Analytics Individual Qualification (IQ)",
                                                    "Content Marketing",
                                                    "Business Branding",
                                                    "Marketing Analytics",
                                                    "Affiliate Marketing"
                                         }
                 },
                {"Health & Fitness", new List<string>{
                                                                    "CBT",
                                                                    "Nutrition",
                                                                    "Yoga",
                                                                    "Herbalism",
                                                                    "Massage",
                                                                    "Fitness",
                                                                    "Art Therapy",
                                                                    "Aromatherapy",
                                                                    "Health Coaching",
                                                                    "Meditation",
                                                                    "Pilates",
                                                                    "Yoga for Kids",
                                                                    "Qi Gong",
                                                                    "Tai Chi",
                                                                    "Dance",
                                                                    "Hypnotherapy",
                                                                    "Dieting",
                                                                    "Medical Terminology",
                                                                    "Weight Loss",
                                                                    "Acupressure"
                                                         }
                },
                {"Business", new List<string>{
                                                "Financial Analysis",
                                                "SQL",
                                                "Investing",
                                                "Project Management",
                                                "Stock Trading",
                                                "Microsoft Power BI",
                                                "Business Analysis",
                                                "Business Fundamentals",
                                                "PMP",
                                                "Financial Modeling",
                                                "Excel",
                                                "Tableau",
                                                "Finance Fundamentals",
                                                "Real Estate Investing",
                                                "Forex",
                                                "Agile",
                                                "PMBOK",
                                                "Amazon FBA",
                                                "Writing",
                                                "Accounting",
                                            }
                },
                {"Personal Development", new List<string>{
                                                                   " Life Coach Training",
                                                                    "Reiki",
                                                                    "Memory",
                                                                    "Speed Reading",
                                                                    "Neuro-Linguistic Programming",
                                                                    "Stock Trading",
                                                                    "Energy Healing",
                                                                    "Mindfulness",
                                                                    "Technical Analysis",
                                                                    "Learning Strategies",
                                                                    "Personal Productivity",
                                                                    "Confidence",
                                                                    "Neuroscience",
                                                                    "Forex",
                                                                    "Spirituality",
                                                                    "Personal Development",
                                                                    "Day Trading",
                                                                    "Public Speaking",
                                                                    "Leadership",
                                                                    "Emotional Intelligence"
                                                             }
                },
                {"Lifestyle", new List<string>{
                                            "Drawing",
                                            "Sketching",
                                            "Watercolor Painting",
                                            "Pencil Drawing",
                                            "Portraiture",
                                            "Figure Drawing",
                                            "Neuro-Linguistic Programming",
                                            "Painting",
                                            "Dog Training",
                                            "Sourdough Bread Baking",
                                            "Acrylic Painting",
                                            "Bread Baking",
                                            "Cooking",
                                            "Procreate Digital Illustration App",
                                            "EFT",
                                            "Oil Painting",
                                            "Soapmaking",
                                            "Day Trading",
                                            "Beauty",
                                            "Makeup Artistry"
                                        }
                },
                 {"Music", new List<string>{
                                                "Piano",
                                                "Guitar",
                                                "Keyboard Instrument",
                                                "Music Production",
                                                "Music Theory",
                                                "Singing",
                                                "Logic Pro X",
                                                "Ableton Live",
                                                "Music Composition",
                                                "FL Studio",
                                                "Music Mixing",
                                                "Songwriting",
                                                "DJ",
                                                "Fingerstyle Guitar",
                                                "Electronic Music",
                                                "Ukulele",
                                                "Blues Guitar",
                                                "Harmonica",
                                                "Audio Production",
                                                "Drums"
                                    }
                 }
            };

            foreach (var devTopic in devTopics)
            {
            //get category that topic/field of interest would be related to.
             var category = await _categoryService.GetCategoryByNameAsync(devTopic.Key);
             //loop through all topics belonging to category
             foreach (var fieldOfInterestName in devTopic.Value)
             {
                 //create a new topic and associate it with a category
                    var topic = new TopicDto()
                    {
                        Name = fieldOfInterestName,
                        CategoryId = category.Data.Id
                    };
                    //persist to database.
                    await _topicService.AddFieldOfInterestAsync(topic);
             }
             
            }
        }

        private static async Task SeedMentorshipRequestAsync(){
            var user = await _userService.GetUserByUsernameAsync("mentor@lifelong.com");
            var request = new FollowDto{
                MenteeUsername = "mentee@lifelong.com",
                MentorUsername = "mentor@lifelong.com",
                TopicName = user.Data.UserFieldOfInterests[0].Topic.Name
            };

            var result = await _followService.CreateMentorshipRequestAsync(request);
        }

        private static async Task SeedFieldOfInterestsForDefaultUsersAsync()
        {
            var usernames = new List<string>{"mentor@lifelong.com", "mentee@lifelong.com"};
            var fieldOfInterests = new List<string>{
                                                        "Web Development",
                                                        "JavaScript",
                                                        "Machine Learning",
                                                        "Java",
                                                        "React",
                                                        "C#"
                                                    };
            var count = 0;
            foreach (var username in usernames)
            {
                var fieldOfInterestName = fieldOfInterests[NewNumber()];
                var request = new UserFieldOfInterestDto
                {
                    Username = username,
                    TopicName = fieldOfInterestName,
                    YearsOfExperience = new Random().Next(1, 5),
                    CurrentlyWorking = false
                };
                //check that user has interest in field
                if(!await _userService.DoesUserHaveInterestInFieldAsync(request)){
                    //add 3 unique field of interest for each user.
                    if (count < 3)
                    {

                        await _userService.AddFieldOfInterestForUser(request);
                    }
                    //increament count variable
                    count++;
                }
            }
            
        }




        private static List<int> randomList = new List<int>();
        private static int NewNumber()
        {
            var MyNumber = new Random().Next(0, 6);
            
            if (!randomList.Contains(MyNumber))
            {
                randomList.Add(MyNumber);
            }
            return MyNumber;
        }
        
    }
}