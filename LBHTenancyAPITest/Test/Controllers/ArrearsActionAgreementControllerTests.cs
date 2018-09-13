using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LBHTenancyAPI.Controllers;
using LBHTenancyAPI.UseCases.ArrearsAgreements;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using AgreementService;
using LBHTenancyAPI.Infrastructure.API;
using LBHTenancyAPI.Infrastructure.UseCase.Execution;

namespace LBHTenancyAPITest.Test.Controllers
{
    public class ArrearsActionAgreementControllerTests
    {
        private ArrearsAgreementController _classUnderTest;
        private Mock<ICreateArrearsAgreementUseCase> _mock;

        public ArrearsActionAgreementControllerTests()
        {
            _mock = new Mock<ICreateArrearsAgreementUseCase>();
            _classUnderTest = new ArrearsAgreementController(_mock.Object);
        }

        [Fact]
        public async Task GivenValidCreateArrearsAgreementRequest_ShouldReturn200()
        {
            //arrange
            _mock.Setup(s => s.ExecuteAsync(It.IsAny<CreateArrearsAgreementRequest>(), CancellationToken.None))
                .ReturnsAsync(new ExecuteWrapper<CreateArrearsAgreementResponse>(new CreateArrearsAgreementResponse
                {
                    
                }));
            var request = new CreateArrearsAgreementRequest();
            //act
            var response = await _classUnderTest.Post(request);
            //assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task WhenGivenIncorrectParameters_AndThereIsErrorFromWebService_ApiShouldRespondWith400()
        {
            //Arrange
            _mock.Setup(s => s.ExecuteAsync(It.IsAny<CreateArrearsAgreementRequest>(), CancellationToken.None))
                .ReturnsAsync(new ExecuteWrapper<CreateArrearsAgreementResponse>(new RequestValidationResponse(false)));
            //Act
            var response = await _classUnderTest.Post(null);

            //Assert);
            Assert.IsType<ObjectResult>(response);
            var responseResult = response as ObjectResult;
            responseResult.StatusCode.Should().Be(400);
            responseResult.Value.Should().NotBeNull();
            var apiResponse = responseResult.Value as APIResponse<CreateArrearsAgreementResponse>;
            apiResponse.Data.Should().BeNull();
            apiResponse.Error.IsValid.Should().BeFalse();
            apiResponse.Error.ValidationErrors.Should().NotBeNull();
        }


        [Fact]
        public async Task WhenGivenCorrectParamaters_AndThereIsExceptionFromWebService_ApiShouldRespondWith500()
        {
            //Arrange

            _mock.Setup(s => s.ExecuteAsync(It.IsAny<CreateArrearsAgreementRequest>(), CancellationToken.None))
               .ReturnsAsync(new ExecuteWrapper<CreateArrearsAgreementResponse>(new Exception("Test Exception")));

            //Act
            var request = new CreateArrearsAgreementRequest
            {
                AgreementInfo = new ArrearsAgreementInfo
                {

                    TenancyAgreementRef = "s",
                    Comment = "testing",
                    ArrearsAgreementStatusCode = "1",
                 
                },
                PaymentSchedule = new List<ArrearsScheduledPaymentInfo>
                {
                    new ArrearsScheduledPaymentInfo
                    {
                        Amount = 10,
                        ArrearsFrequencyCode = "200",
                        Comments = "testing",
                       
                    }
                }.ToArray()
            };

            var response = await _classUnderTest.Post(request);

            //Assert
            //Assert);
            Assert.IsType<ObjectResult>(response);
            var responseResult = response as ObjectResult;
            responseResult.StatusCode.Should().Be(500);
            responseResult.Value.Should().NotBeNull();
            var apiResponse = responseResult.Value as APIResponse<CreateArrearsAgreementResponse>;
            apiResponse.Data.Should().BeNull();
            apiResponse.Error.ValidationErrors.Should().NotBeNull();
        }


        [Fact]
        public async Task WhenGivenCorrectParamaters_AndThereIsErrorFromWebService_ApiShouldRespondWith500()
        {
            //Arrange

            _mock.Setup(s => s.ExecuteAsync(It.IsAny<CreateArrearsAgreementRequest>(), CancellationToken.None))
               .ReturnsAsync(new ExecuteWrapper<CreateArrearsAgreementResponse>(new ExecutionError()));

            //Act
            var request = new CreateArrearsAgreementRequest
            {
                AgreementInfo = new ArrearsAgreementInfo
                {

                    TenancyAgreementRef = "s",
                    Comment = "testing",
                    ArrearsAgreementStatusCode = "1",

                },
                PaymentSchedule = new List<ArrearsScheduledPaymentInfo>
                {
                    new ArrearsScheduledPaymentInfo
                    {
                        Amount = 10,
                        ArrearsFrequencyCode = "200",
                        Comments = "testing",

                    }
                }.ToArray()
            };

            var response = await _classUnderTest.Post(request);

            //Assert
            //Assert);
            Assert.IsType<ObjectResult>(response);
            var responseResult = response as ObjectResult;
            responseResult.StatusCode.Should().Be(500);
            responseResult.Value.Should().NotBeNull();
            var apiResponse = responseResult.Value as APIResponse<CreateArrearsAgreementResponse>;
            apiResponse.Data.Should().BeNull();
            apiResponse.Error.ValidationErrors.Should().NotBeNull();
        }

    }
}
