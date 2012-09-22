Made in Visual Studio 2010

The purpose of this application is to show a simple implementation of the Smith-Waterman alignment.

Usage: 
Form contains two modifiable textboxes for input. The button_click event will perform the alignment and write into the bottom textboxes.

To be improved:
Use a stack for sequence building (no reason to use a list collection if we use a FILO structure)
Make the application prettier
Don't waste time in having variables that only need to be used once
Dropdowns for selecting gap/match and affine gap extension coefficients
Rename variables for the sake of legibility and not being stupid ("int gapgrower"???)