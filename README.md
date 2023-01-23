# HACC MDS Joiner

A windows application that merges two or more Victorian HACC MDS files.

 ![Screenshot](https://github.com/DMCCV/HACC-MDS-Joiner/blob/master/Resources/Screenshot.png)

## Instructions

1. Open a primary HACC MDS file, this should contain the correct agency identifier, reporting identifier, and be named as per the MDS specifications. The application only accepts CSV files.
2. Open subsequent files from other sources and repeat as nessecary. The application won't allow the same file to be added twice.
3. When finished, save the resulting file. The file will be ready for submission

## How it works

- Header information, such as agency identifier and reporting date, is taken from the primary file. Any header information in secondary files is ignored.
- When a secondary file is added all client data is added to the output file.
- When a duplicate client SLK is encountered it will merge the client records into a single row:
  - The values from the primary file will be retained, except for when the primary file field is  empty or 'not stated', in this case the value from the secondary file will be substituted.
  - Where the value is numerical (e.g. hours, dollars) the values from the secondary files will be added to the primary.
- The output window will display information, errors and warnings for each step.
- The primary file name is retained when the output file is saved, however it can be altered in the save file dialog.

## Download

Version 1.0

[HACC MDS Joiner - Installer.msi](https://github.com/DMCCV/HACC-MDS-Joiner/releases/download/v1.0/HACC.MDS.Joiner.-.Installer.msi)

[HACC MDS Joiner - Portable.zip](https://github.com/DMCCV/HACC-MDS-Joiner/releases/download/v1.0/HACC.MDS.Joiner.-.Portable.zip)

## More Information

This tool is based on version 2.0.1 of the Victorian HACC MDS, documentation can be found here:
https://www.health.vic.gov.au/home-and-community-care/reporting-and-data





 

 
