using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LBHTenancyAPI.Gateways.Search;
using LBHTenancyAPI.UseCases.Contacts.Models;
using LBHTenancyAPITest.Helpers;
using Xunit;
using LBHTenancyAPITest.Helpers.Data;

namespace LBHTenancyAPITest.Test.Gateways.Search
{
    public class SearchGatewayTests : IClassFixture<DatabaseFixture>
    {
        readonly SqlConnection _db;
        private readonly ISearchGateway _classUnderTest;

        public SearchGatewayTests(DatabaseFixture fixture)
        {
            _db = fixture.Db;
            var connection = DotNetEnv.Env.GetString("UH_CONNECTION_STRING");
            _classUnderTest = new SearchGateway(connection);
        }

        [Theory]
        [InlineData("Smith")]
        [InlineData("Shetty")]
        public async Task can_search_on_last_name(string lastName)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            expectedMember.surname = lastName;
            TestDataHelper.InsertMember(expectedMember, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = lastName,
                PageSize = 10,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.Results[0].PrimaryContactName.Should().Contain(lastName);
        }

        [Theory]
        [InlineData("Jeff")]
        [InlineData("Rashmi")]
        public async Task can_search_on_first_name(string firstName)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            expectedMember.forename = firstName;
            TestDataHelper.InsertMember(expectedMember, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = firstName,
                PageSize = 10,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.Results[0].PrimaryContactName.Should().Contain(firstName);
        }

        [Theory]
        [InlineData("E8 1HH")]
        [InlineData("E8 1EA")]
        public async Task can_search_on_post_code(string postCode)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            expectedProperty.post_code = postCode;
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            
            TestDataHelper.InsertMember(expectedMember, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = postCode,
                PageSize = 10,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.Results[0].PrimaryContactPostcode.Should().Contain(postCode);
        }

        [Theory]
        [InlineData("17 Reading Lane")]
        [InlineData("Hackney Contact Center")]
        public async Task can_search_on_short_address(string shortAddress)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            expectedProperty.short_address = shortAddress;
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            TestDataHelper.InsertMember(expectedMember, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = shortAddress,
                PageSize = 10,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.Results[0].PrimaryContactShortAddress.Should().Contain(shortAddress);
        }

        [Theory]
        [InlineData("000017/01")]
        [InlineData("000018/01")]
        public async Task can_search_on_tenancy_reference(string tagRef)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.tag_ref = tagRef;
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            TestDataHelper.InsertMember(expectedMember, _db);
            //arrears agreement
            var expectedArrearsAgreement = Fake.UniversalHousing.GenerateFakeArrearsAgreement();
            expectedArrearsAgreement.tag_ref = expectedTenancy.tag_ref;
            TestDataHelper.InsertAgreement(expectedArrearsAgreement, _db);
            //arrears agreement det
            var expectedArrearsAgreementDet = Fake.UniversalHousing.GenerateFakeArrearsAgreementDet();
            expectedArrearsAgreementDet.tag_ref = expectedTenancy.tag_ref;
            TestDataHelper.InsertAgreementDet(expectedArrearsAgreementDet, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = tagRef,
                PageSize = 10,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.Results[0].TenancyRef.Should().Contain(tagRef);
        }

        [Theory]
        [InlineData("Smith")]
        [InlineData("Shetty")]
        public async Task can_get_total_count(string lastName)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member 1
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            expectedMember.surname = lastName;
            TestDataHelper.InsertMember(expectedMember, _db);
            //member 2
            var expectedMember2 = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember2.house_ref = expectedTenancy.house_ref;
            expectedMember2.surname = lastName;
            TestDataHelper.InsertMember(expectedMember, _db);

            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = lastName,
                PageSize = 1,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.TotalResultsCount.Should().Be(2);
        }

        [Theory]
        [InlineData("Brady")]
        [InlineData("Donaldson")]
        public async Task can_search_even_with_no_arrears_agreement(string lastName)
        {
            //arrange
            //property
            var expectedProperty = Fake.UniversalHousing.GenerateFakeProperty();
            TestDataHelper.InsertProperty(expectedProperty, _db);
            //tenancy
            var expectedTenancy = Fake.UniversalHousing.GenerateFakeTenancy();
            expectedTenancy.house_ref = expectedTenancy.house_ref;
            expectedTenancy.prop_ref = expectedProperty.prop_ref;
            TestDataHelper.InsertTenancy(expectedTenancy, _db);
            //member 1
            var expectedMember = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember.house_ref = expectedTenancy.house_ref;
            expectedMember.surname = lastName;
            TestDataHelper.InsertMember(expectedMember, _db);
            //member 2
            var expectedMember2 = Fake.UniversalHousing.GenerateFakeMember();
            expectedMember2.house_ref = expectedTenancy.house_ref;
            expectedMember2.surname = lastName;
            TestDataHelper.InsertMember(expectedMember2, _db);
            //act
            var response = await _classUnderTest.SearchTenanciesAsync(new SearchTenancyRequest
            {
                SearchTerm = lastName,
                PageSize = 1,
                Page = 0
            }, CancellationToken.None);
            //assert
            response.Should().NotBeNull();
            response.Results.Should().NotBeNullOrEmpty();
            response.TotalResultsCount.Should().Be(2);
        }
    }
}