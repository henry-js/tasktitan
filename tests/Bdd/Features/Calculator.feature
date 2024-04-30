Feature: Calculator
Simple calculator for adding two numbers

  @mytag
  Scenario: Add two numbers
    Given the first number is 50
    And the second number is 70
    When the two numbers are added
    Then the result should be 120

  Scenario: I want to clear my list
    Given the following users exist:
      | description     |
      | Walk the dog    |
      | Feed the cats   |
      | Put the bin out |
	When I clear the list
	Then there should be no due tasks
