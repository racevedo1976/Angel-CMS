using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Angelo.Connect.Web.Data.Mock
{
    public static class MockData
    {
        public static List<Person> People { get; set; }

        public static List<MockFile> Files { get; set; }

        public static List<MockFolder> Folders { get; set; }

        public static List<MockMessage> Messages { get; set; }

        public static List<MockMessageCategory> MessageCategories { get; set; }

        static MockData()
        {
            People = DeserializeList<Person>("People.json");
            Messages = InitMessages();
            MessageCategories = InitMessageCategories();
            Folders = InitFolders();
            Files = InitFiles();
        }

        private static List<TEntity> DeserializeList<TEntity>(string fileName)
        {
            var jsonData = GetEmbeddedResourceString(fileName);

            return JsonConvert.DeserializeObject<List<TEntity>>(jsonData);
        }

        private static string GetEmbeddedResourceString(string fileName)
        {
            var assembly = typeof(MockData).GetTypeInfo().Assembly;
            var jsonPath = typeof(MockData).FullName.Replace(".Mock.MockData", ".Json.MockData");

            var stream = assembly.GetManifestResourceStream(jsonPath + "." + fileName);
            string contents = "";

            using (var reader = new StreamReader(stream))
            {
                contents = reader.ReadToEnd();
            }

            return contents;
        }

        private static List<MockMessage> InitMessages()
        {
            return new List<MockMessage>()
            {
                new MockMessage { Title = "Hi, Welcome to the system", CategoryName = "System", TimeStamp = DateTime.Parse("6/20/2018 11:05 AM") },
                new MockMessage { Title = "Congrats on your first blog post!", CategoryName = "Blog", TimeStamp = DateTime.Parse("6/21/2018 1:30 PM") },
                new MockMessage { Title = "You have new comments on your blog", CategoryName = "Blog", TimeStamp = DateTime.Parse("6/21/2018 2:10 PM") },
                new MockMessage { Title = "Susan shared a category with you", CategoryName = "Blog", TimeStamp = DateTime.Parse("6/25/2018 3:58 PM") },
            };
        }

        private static List<MockMessageCategory> InitMessageCategories()
        {
            var categories = new List<MockMessageCategory>();
            var messages = Messages ?? InitMessages();

            foreach (var categoryName in messages.Select(x => x.CategoryName).Distinct())
            {
                categories.Add(new MockMessageCategory
                {
                    CategoryName = categoryName,
                    MessageCount = messages.Where(x => x.CategoryName == categoryName).Count()
                });
            }

            return categories;
        }

        private static List<MockFolder> InitFolders()
        {
            return new List<MockFolder>()
            {
              new MockFolder {
                Id = "folder-100",
                Name = "Images",
                VirtualPath = "/img",
                ParentId = null
              },
              new MockFolder {
                Id = "folder-110",
                Name = "Paintings",
                VirtualPath = "/img/paintings",
                ParentId = "folder-100"
              },
              new MockFolder {
                Id = "folder-120",
                Name = "Avatars",
                VirtualPath = "/img/avatars",
                ParentId = "folder-100"
              },
              new MockFolder {
                Id = "folder200",
                Name = "Videos",
                VirtualPath = "/videos",
                ParentId = null
              },
              new MockFolder {
                Id = "folder300",
                Name = "Documents",
                VirtualPath = "/documents",
                ParentId = null
              }
            };
        }

        private static List<MockFile> InitFiles()
        {
            return new List<MockFile>()
            {
              new MockFile {
                Id = "file-001",
                Name = "painting01.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting01.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-002",
                Name = "painting02.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting02.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-003",
                Name = "painting03.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting03.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-004",
                Name = "painting04.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting04.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-005",
                Name = "painting05.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting05.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-006",
                Name = "painting06.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting06.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-007",
                Name = "painting07.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting07.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-008",
                Name = "painting08.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting08.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-009",
                Name = "painting09.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting09.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-010",
                Name = "painting10.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting10.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-011",
                Name = "painting11.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting11.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-012",
                Name = "painting12.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting12.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-013",
                Name = "painting13.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting13.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-014",
                Name = "painting14.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting14.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-015",
                Name = "painting15.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting15.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-016",
                Name = "painting16.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting16.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-017",
                Name = "painting17.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting17.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-018",
                Name = "painting18.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting18.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-019",
                Name = "painting19.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting19.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-020",
                Name = "painting20.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting20.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-021",
                Name = "painting21.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting21.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-022",
                Name = "painting22.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting22.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-023",
                Name = "painting23.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting23.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-024",
                Name = "painting24.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting24.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-025",
                Name = "painting25.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting25.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-026",
                Name = "painting26.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting26.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-027",
                Name = "painting27.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting27.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-028",
                Name = "painting28.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting28.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file29",
                Name = "painting29.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting29.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-030",
                Name = "painting30.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting30.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-031",
                Name = "painting31.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting31.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-032",
                Name = "painting32.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting32.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-033",
                Name = "painting33.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting33.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-034",
                Name = "painting34.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting34.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-035",
                Name = "painting35.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting35.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-036",
                Name = "painting36.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting36.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-037",
                Name = "painting37.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting37.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-038",
                Name = "painting38.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting38.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-039",
                Name = "painting39.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting39.jpg",
                FolderId = "folder-110"
              },
              new MockFile {
                Id = "file-040",
                Name = "painting40.jpg",
                ContentType = "image/jpeg",
                PhysicalPath = "/img/oil/painting40.jpg",
                FolderId = "folder-110"
              }
            };
        }
    }
}
