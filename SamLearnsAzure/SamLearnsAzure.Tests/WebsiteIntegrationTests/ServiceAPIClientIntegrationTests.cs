using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Controllers;

namespace SamLearnsAzure.Tests.WebsiteIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("WebIntegrationTest")]
    public class ServiceApiClientIntegrationTests : BaseIntegrationTest
    {

        [TestMethod]
        public async Task GetOwnersServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Owners> owners = await client.GetOwners();

            //Assert
            Assert.IsTrue(owners.Any());
        }

        [TestMethod]
        public async Task GetOwnerSetsServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            int ownerId = 1;

            //Act
            List<OwnerSets> ownerSets = await client.GetOwnerSets(ownerId);

            //Assert
            Assert.IsTrue(ownerSets.Any());
        }

        [TestMethod]
        public async Task GetSetsServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Sets> sets = await client.GetSets();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Any());
        }

        [TestMethod]
        public async Task GetSetServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            Sets set = await client.GetSet(setNum);

            //Assert
            Assert.IsTrue(set != null);
        }

        [TestMethod]
        public async Task GetSetImageServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75218-1";

            //Act
            SetImages setImage = await client.GetSetImage(setNum);

            //Assert
            Assert.IsTrue(setImage != null);
        }

        //[TestMethod]
        //public async Task GetSetPartsServiceApiIntegrationTest()
        //{
        //    //Arrange
        //    ServiceApiClient client = new ServiceApiClient(base.Configuration);
        //    string setNum = "75218-1";

        //    //Act
        //    List<SetParts> setParts = await client.GetSetParts(setNum);

        //    //Assert
        //    Assert.IsTrue(setParts.Any());
        //}

        [TestMethod]
        public async Task GetThemesServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<Themes> themes = await client.GetThemes();

            //Assert
            Assert.IsTrue(themes.Any());
        }

        [TestMethod]
        public async Task GetPartImagesServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);

            //Act
            List<PartImages> partImages = await client.GetPartImages();

            //Assert
            Assert.IsTrue(partImages.Any());
        }

        //[TestMethod]
        //public async Task GetSetImagesServiceApiIntegrationTest()
        //{
        //    //Arrange
        //    ServiceApiClient client = new ServiceApiClient(base.Configuration);
        //    string setNum = "75218-1";
        //    int resultsToReturn = 2;
        //    int resultsToSearch = 4;

        //    //Act
        //    List<SetImages> setImages = await client.GetSetImages(setNum, resultsToReturn, resultsToSearch);

        //    //Assert
        //    Assert.IsTrue(setImages.Any());
        //}

        //[TestMethod]
        //public async Task SaveSetImageServiceApiIntegrationTest()
        //{
        //    //Arrange
        //    ServiceApiClient client = new ServiceApiClient(base.Configuration);
        //    string setNum = "75218-2";
        //    string imageUrl = "https://samlearnsazure.files.wordpress.com/2019/01/microsoft-certified-azure-solutions-architect-expert.png";

        //    //Act
        //    SetImages setImage = await client.SaveSetImage(setNum, imageUrl);

        //    //Assert
        //    Assert.IsTrue(setImage != null);
        //}

        [TestMethod]
        public async Task SearchForMissingPartsServiceApiIntegrationTest()
        {
            //Arrange
            ServiceApiClient client = new ServiceApiClient(base.Configuration);
            string setNum = "75168-1"; //Yoda's Jedi starfighter

            //Act
            bool result = await client.SearchForMissingParts(setNum);

            //Assert
            Assert.IsTrue(result);
        }

    }
}
