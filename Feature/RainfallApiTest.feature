Feature: Rainfall API Enhancement Testing

  Scenario: Retrieve limited rainfall measurements for a station
    Given the rainfall API is available
    When I request the rainfall measurements for station "52203" with a limit of 5
    Then I should receive 5 measurements

  Scenario: Retrieve rainfall measurements for a specific date
    Given the rainfall API is available
    When I request the rainfall measurements for station "52203" on "2024-07-09"
    Then I should receive measurements only for "2024-07-09"
