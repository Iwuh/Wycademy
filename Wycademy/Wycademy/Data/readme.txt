Generating JSON files:

1. Download the latest spreadsheet from https://docs.google.com/spreadsheets/d/1avFYGBP_J6BYpxsn3W25g98MJ4ijJeHLdXLWY-lrM_0/edit?usp=sharing
2. Run "generate_json.py"
3. JSON files will be in the json directory.

Structure of the spreadsheet is as follows:

Each monster has it's own sheet.
Each sheet has 3 columns: Category, Key, and Value.
These are used to organize the data when generating the JSON files.
Sometimes, Value will have multiple numbers.
The bot will split them up when parsing the data.

What multiple numbers in the value column refer to:

Hitzone: Cut/Impact/Shot/Fire/Water/Ice/Thunder/Dragon/KO/Exhaust
Stagger/Sever: Stagger Value/Sever Value/Extract Colour
Status: Initial/Increase/Max/Duration/Reduction/Damage
Item Effects: Duration Normal/Duration Enraged/Duration Fatigued