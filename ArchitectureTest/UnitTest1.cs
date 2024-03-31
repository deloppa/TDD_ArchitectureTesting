using Architecture.Controllers;
using Architecture.Repositories;
using Architecture.Repositories.Interfaces;
using Architecture.Services;
using Architecture.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace ArchitectureTest;

public class UnitTest1
{
    private readonly string _controllersNamespace = "Architecture.Controllers";
    private readonly string _servicesNamespace = "Architecture.Services";
    private readonly string _repositoriesNamespace = "Architecture.Repositories";
    private readonly string _servicesInterfacesNamespace = "Architecture.Services.Interfaces";
    private readonly string _repositoriesInterfacesNamespace = "Architecture.Repositories.Interfaces";
    private readonly string _modelsNamespace = "Architecture.Models";
    private readonly ITestOutputHelper _output;

    private readonly Assembly assembly = typeof(ItemController).Assembly;


    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Controllers_ShouldDependOnServices_AndNotOnRepositories()
    {
        var t = Types.InAssembly(assembly).GetTypes();

        foreach (var item in t)
        {
            _output.WriteLine(item.FullName);
        }

        var result = Types.InAssembly(assembly)
        .That().ResideInNamespace(_controllersNamespace)
        .Should()
        .HaveDependencyOn(_servicesNamespace)
        .And()
        .NotHaveDependencyOn(_repositoriesNamespace)
        .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Services_ShouldDependOnRepositories_AndNotOnControllers()
    {
        var result = Types.InAssembly(assembly)
        .That().ResideInNamespace(_servicesNamespace)
        .And().AreClasses()
        .Should().HaveDependencyOn(_repositoriesInterfacesNamespace)
        .And().NotHaveDependencyOn(_controllersNamespace)
        .GetResult();

        if (!result.IsSuccessful)
        {
            foreach (var violation in result.FailingTypeNames)
            {
                _output.WriteLine(violation);
            }
        }

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Repository_ShouldNotDependOnControllersAndServices()
    {
        var result = Types.InAssembly(assembly)
        .That()
        .ResideInNamespace(_repositoriesNamespace)
        .Should().NotHaveDependencyOn(_servicesInterfacesNamespace)
        .And().NotHaveDependencyOn(_controllersNamespace)
        .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void ControllerClassesNameHasToEndOnController()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_controllersNamespace)
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void ServiceClassesNameHasToEndOnService()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_servicesNamespace)
            .Should()
            .HaveNameEndingWith("Service")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void RepositoryClassesNameHasToEndOnRepository()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_repositoriesNamespace)
            .And().AreClasses()
            .Should().HaveNameEndingWith("Repository")
            .Or().HaveNameStartingWith("Generic")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CheckControllerAnnotations()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_controllersNamespace)
            .Should().HaveCustomAttribute(typeof(ApiControllerAttribute))
            .And().HaveCustomAttribute(typeof(RouteAttribute))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CheckIfClassesArePublic()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_controllersNamespace)
            .And().ResideInNamespace(_servicesNamespace)
            .And().ResideInNamespace(_repositoriesNamespace)
            .And().ResideInNamespace(_modelsNamespace)
            .And().AreClasses()
            .Should().BePublic()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CheckIfInterfacesArePublic()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_servicesNamespace)
            .And().ResideInNamespace(_repositoriesNamespace)
            .And().ResideInNamespace(_modelsNamespace)
            .And().AreInterfaces()
            .Should().BePublic()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CheckIfControllerInheritControllerBaseInterface()
    {
        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(_controllersNamespace)
            .Should()
            .Inherit(typeof(ControllerBase))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void CheckIfServicesImplementSameNameInterface()
    {
        var serviceTypes = Types.InAssembly(assembly)
                .That().ResideInNamespace(_servicesNamespace)
                .And().AreClasses()
                .GetTypes();

        foreach (var serviceType in serviceTypes)
        {
            var interfaces = serviceType.GetInterfaces();
            var hasMatchingInterface = interfaces.Any(i => i.Name.StartsWith("I") && i.Name.Substring(1) == serviceType.Name);
            Assert.True(hasMatchingInterface);
        }
    }

    [Fact]
    public void CheckIfRepositoriesImplementSameNameInterface()
    {
        var repositoryTypes = Types.InAssembly(assembly)
                .That().ResideInNamespace(_repositoriesNamespace)
                .And().AreClasses()
                .GetTypes();

        foreach (var repositoryType in repositoryTypes)
        {
            var interfaces = repositoryType.GetInterfaces();
            var hasMatchingInterface = interfaces.Any(i => i.Name.StartsWith("I") && i.Name.Substring(1) == repositoryType.Name);
            Assert.True(hasMatchingInterface);

        }
    }

    [Fact]
    public void CheckIfModelsHaveId()
    {
        var typesInNamespace = Types.InAssembly(assembly)
            .That().ResideInNamespace(_modelsNamespace)
            .GetTypes();

        foreach (var type in typesInNamespace)
        {
            var idProperty = type.GetProperties().FirstOrDefault(p => p.Name.Equals("Id"));
            Assert.NotNull(idProperty);
        }
    }

    [Fact]
    public void CheckIfModelsHaveName()
    {
        var typesInNamespace = Types.InAssembly(assembly)
            .That().ResideInNamespace(_modelsNamespace)
            .GetTypes();

        foreach (var type in typesInNamespace)
        {
            var idProperty = type.GetProperties().FirstOrDefault(p => p.Name.Equals("Name"));
            Assert.NotNull(idProperty);
        }
    }

    [Fact]
    public void CheckIfModelsHaveDescription()
    {
        var typesInNamespace = Types.InAssembly(assembly)
            .That().ResideInNamespace(_modelsNamespace)
            .GetTypes();

        foreach (var type in typesInNamespace)
        {
            var idProperty = type.GetProperties().FirstOrDefault(p => p.Name.Equals("Description"));
            Assert.NotNull(idProperty);
        }
    }

    [Fact]
    public void OneRepositoryShouldBeGeneric()
    {
        var hasGenericRepository = Types.InAssembly(assembly)
            .That().ResideInNamespace(_repositoriesNamespace)
            .And().AreClasses()
            .And().ImplementInterface(typeof(IGenericRepository<>))
            .GetTypes()
            .Any(t => t.IsGenericType);

        Assert.True(hasGenericRepository);
    }

    [Fact]
    public void ShouldContainGenericInterfaces()
    {
        var types = Types.InAssembly(assembly)
            .That().ResideInNamespace(_repositoriesNamespace)
            .GetTypes();

        var hasGenericInterfaces = types.Any(type =>
            type.IsInterface && type.IsGenericType);

        Assert.True(hasGenericInterfaces);
    }

    [Fact]
    public void NonGenericRepositoriesShouldExtendGenericRepository()
    {
        var types = Types.InAssembly(assembly)
            .That().ResideInNamespace(_repositoriesNamespace)
            .GetTypes();

        var nonGenericRepositories = types.Where(type =>
            type.IsClass && !type.IsGenericType);

        foreach (var repository in nonGenericRepositories)
        {
            var baseType = repository.BaseType;
            var isExtendingGenericRepository = baseType?.IsGenericType == true &&
                                                baseType.GetGenericTypeDefinition() == typeof(GenericRepository<>);
            Assert.True(isExtendingGenericRepository);
        }
    }

}