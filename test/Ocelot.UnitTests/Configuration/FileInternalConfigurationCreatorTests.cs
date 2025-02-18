﻿namespace Ocelot.UnitTests.Configuration
{
    using Moq;
    using Ocelot.Configuration;
    using Ocelot.Configuration.Builder;
    using Ocelot.Configuration.Creator;
    using Ocelot.Configuration.File;
    using Ocelot.Configuration.Validator;
    using Ocelot.Errors;
    using Ocelot.Responses;
    using Ocelot.UnitTests.Responder;
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TestStack.BDDfy;
    using Xunit;

    public class FileInternalConfigurationCreatorTests
    {
        private readonly Mock<IConfigurationValidator> _validator;
        private readonly Mock<IReRoutesCreator> _reRoutesCreator;
        private readonly Mock<IAggregatesCreator> _aggregatesCreator;
        private readonly Mock<IDynamicsCreator> _dynamicsCreator;
        private readonly Mock<IConfigurationCreator> _configCreator;
        private Response<IInternalConfiguration> _config;
        private FileConfiguration _fileConfiguration;
        private readonly FileInternalConfigurationCreator _creator;
        private Response<IInternalConfiguration> _result;
        private List<ReRoute> _reRoutes;
        private List<ReRoute> _aggregates;
        private List<ReRoute> _dynamics;
        private InternalConfiguration _internalConfig;

        public FileInternalConfigurationCreatorTests()
        {
            _validator = new Mock<IConfigurationValidator>();
            _reRoutesCreator = new Mock<IReRoutesCreator>();
            _aggregatesCreator = new Mock<IAggregatesCreator>();
            _dynamicsCreator = new Mock<IDynamicsCreator>();
            _configCreator = new Mock<IConfigurationCreator>();

            _creator = new FileInternalConfigurationCreator(_validator.Object, _reRoutesCreator.Object, _aggregatesCreator.Object, _dynamicsCreator.Object, _configCreator.Object);
        }

        [Fact]
        public void should_return_validation_error()
        {
            var fileConfiguration = new FileConfiguration();

            this.Given(_ => GivenThe(fileConfiguration))
                .And(_ => GivenTheValidationFails())
                .When(_ => WhenICreate())
                .Then(_ => ThenAnErrorIsReturned())
                .BDDfy();
        }

        [Fact]
        public void should_return_internal_configuration()
        {
            var fileConfiguration = new FileConfiguration();

            this.Given(_ => GivenThe(fileConfiguration))
                .And(_ => GivenTheValidationSucceeds())
                .And(_ => GivenTheDependenciesAreSetUp())
                .When(_ => WhenICreate())
                .Then(_ => ThenTheDependenciesAreCalledCorrectly())
                .BDDfy();
        }

        private void ThenTheDependenciesAreCalledCorrectly()
        {
            _reRoutesCreator.Verify(x => x.Create(_fileConfiguration), Times.Once);
            _aggregatesCreator.Verify(x => x.Create(_fileConfiguration, _reRoutes), Times.Once);
            _dynamicsCreator.Verify(x => x.Create(_fileConfiguration), Times.Once);

            var mergedReRoutes = _reRoutes
                .Union(_aggregates)
                .Union(_dynamics)
                .ToList();

            _configCreator.Verify(x => x.Create(_fileConfiguration, It.Is<List<ReRoute>>(y => y.Count == mergedReRoutes.Count)), Times.Once);
        }

        private void GivenTheDependenciesAreSetUp()
        {
            _reRoutes = new List<ReRoute> { new ReRouteBuilder().Build() };
            _aggregates = new List<ReRoute> { new ReRouteBuilder().Build() };
            _dynamics = new List<ReRoute> { new ReRouteBuilder().Build() };
            _internalConfig = new InternalConfiguration(null, "", null, "", null, "", null, null);

            _reRoutesCreator.Setup(x => x.Create(It.IsAny<FileConfiguration>())).Returns(_reRoutes);
            _aggregatesCreator.Setup(x => x.Create(It.IsAny<FileConfiguration>(), It.IsAny<List<ReRoute>>())).Returns(_aggregates);
            _dynamicsCreator.Setup(x => x.Create(It.IsAny<FileConfiguration>())).Returns(_dynamics);
            _configCreator.Setup(x => x.Create(It.IsAny<FileConfiguration>(), It.IsAny<List<ReRoute>>())).Returns(_internalConfig);
        }

        private void GivenTheValidationSucceeds()
        {
            var ok = new ConfigurationValidationResult(false);
            var response = new OkResponse<ConfigurationValidationResult>(ok);
            _validator.Setup(x => x.IsValid(It.IsAny<FileConfiguration>())).ReturnsAsync(response);
        }

        private void ThenAnErrorIsReturned()
        {
            _result.IsError.ShouldBeTrue();
        }

        private async Task WhenICreate()
        {
            _result = await _creator.Create(_fileConfiguration);
        }

        private void GivenTheValidationFails()
        {
            var error = new ConfigurationValidationResult(true, new List<Error> { new AnyError() });
            var response = new OkResponse<ConfigurationValidationResult>(error);
            _validator.Setup(x => x.IsValid(It.IsAny<FileConfiguration>())).ReturnsAsync(response);
        }

        private void GivenThe(FileConfiguration fileConfiguration)
        {
            _fileConfiguration = fileConfiguration;
        }
    }
}
