# FileLineValidator
 Reads lines from a text file and validates them

 We need Web API solution for a data file validation. This solution should have 1 endpoint to upload and validate a file. File content is a newline separated list of account names and numbers (separated by a single space), which in case of being valid are actual bank account owner first names and account numbers.
Example content of the file is this:
Thomas 32999921
Richard 3293982
XAEA-12 8293982
Rose 329a982
Bob 329398.
michael 3113902
Rob 3113902p
Validation rules are:
1. First name should only consist of alphabetic characters (any symbol and numeric characters are not allowed), first letter should always be an uppercase
2. Account number is a 7 digit number (ex. 3293982) or 7 digit number + 'p' at the end (4113902p)
3. Valid account number must start with a digit 3 or 4 
Expected result after processing file should be:
1. In case of valid file - Web API gives a simple JSON response with content { fileValid: true }
2. In case at least single line is not valid JSON response should return fileValid = false and list of invalid lines with explanations, line number and actual line content. 
Example file response result would be:
{ fileValid: false,
invalidLines: [
              "Account number - not valid for 1 line 'Thomas 32999921'",
              "Account name, account number - not valid for 3 line 'XAEA-12 8293982'",
              "Account number - not valid for 4 line 'Rose 329a982'",
              "Account number - not valid for 5 line 'Bob 329398.'",
              "Account name - not valid for 6 line 'michael 3113902'"
]
} 
Requirements:
1. Up-to-date .NET version project
2. Business service(s) with unit tests
3. At least one validation should be handled using Regex library
4. Put source code in GIT.
5. Use best software engineering practices while developing this application 
Bonus points:
1. Time performance is measured between line validations, so an engineer could differentiate which lines where the slowest and which were fastest to validate.
