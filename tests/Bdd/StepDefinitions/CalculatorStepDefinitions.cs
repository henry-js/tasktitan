using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestPlatform.TestHost;

using Reqnroll;

namespace Bdd.StepDefinitions;

[Binding]
public sealed class CalculatorStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;


    public CalculatorStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

    [Given("the first number is {int}")]
    public void GivenTheFirstNumberIs(int number)
    {
        //TODO: implement arrange (precondition) logic
        // For storing and retrieving scenario-specific data see https://go.reqnroll.net/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/DataTable parameters can be defined on the step definition
        // method.

        var tasks = _scenarioContext.Get<List<string>>();
    }

    [Given("the second number is {int}")]
    public void GivenTheSecondNumberIs(int number)
    {
        //TODO: implement arrange (precondition) logic

        throw new PendingStepException();
    }

    [When("the two numbers are added")]
    public void WhenTheTwoNumbersAreAdded()
    {
        //TODO: implement act (action) logic

        throw new PendingStepException();
    }

    [Then("the result should be {int}")]
    public void ThenTheResultShouldBe(int result)
    {
        //TODO: implement assert (verification) logic

        throw new PendingStepException();
    }

    // This step definition uses Cucumber Expressions. See https://github.com/gasparnagy/CucumberExpressions.SpecFlow
    [Given("the following users exist:")]
    public void GivenTheFollowingUsersExist()
    {
        // Write code here that turns the phrase above into concrete actions
    }

    // This step definition uses Cucumber Expressions. See https://github.com/gasparnagy/CucumberExpressions.SpecFlow
    [When("I clear the list")]
    public void WhenIClearTheList()
    {
        // Write code here that turns the phrase above into concrete actions
    }

    // This step definition uses Cucumber Expressions. See https://github.com/gasparnagy/CucumberExpressions.SpecFlow
    [Then("there should be no due tasks")]
    public void ThenThereShouldBeNoDueTasks()
    {
        // Write code here that turns the phrase above into concrete actions
    }
}
