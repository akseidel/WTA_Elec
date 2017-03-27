# WTA_Elec

![RibbonTab](/ElecRibbonTab.PNG)

Revit Add-in in c# &mdash; Creates a custom ribbon Tab with discipline related tools. The tools at this writing are for placing specific Revit family types with some families requiring parameter settings made on the fly.

This repository is provided for sharing and learning purposes. Perhaps someone might provide improvements or education. Perhaps it will help to boost someone further up the steep learning curve needed to create Revit task add-ins. Hopefully it does not show too much of the wrong way.  

Used by the tools in this ribbon are classes intended to provide a Revit family instance placement without the Revit user interface overhead normally required by Revit. The classes are intended to provide a universal mechanism for placing some types of Revit families. This includes tags, which is a task not in this discipline tab but is in other discipline add-ins. The custom tab employs menu methods not commonly explained, for example a split button sets a family placement mode that is exposed to the functions called by command picks. Other tools use add-in application settings as a way to persist settings or communicate to code that runs subsequently within a command that provides a task workflow.

This add-in demonstrates many of the typical tasks and implementation required for providing a tab menu interface involving family placement, e.g.:

* Creating a ribbon tab populated with some controls
  - Tool tips
  - Image file to button image
  - Communication between controls and commands the controls execute
* Establishing the family type for placement
  - Determine if the correct pairing exists in the current file
  - Automatically discovering and loading the family if it does not exist in the current file but does exist somewhere starting from some set directory
* Providing the family type placement interface
  - In multiple mode or one shot mode
  - With a heads-up status/instruction interface form
    - As WPF with independent behavior
      - Sending focus back to the Revit view
  - Returning the family instance placement for further processing after the instance has been placed
  - Managing an escape out from the process
  - Handling correct view type context
* Changing family parameter values

Much of the code is by others. Its mangling and ignorant misuse is my ongoing doing. Much thanks to the professionals like Jeremy Tammik who provided the means directly or by mention one way or another for probably all the code needed.


One task demonstrated shows rooms being selected with lighting information for that room being shown on a floating WPF. The information contains data not passed through light fixture load connectors and therefore cannot be seen via schedules.

![SensorField](/WTA_Elec/SENSDETLG.PNG)

Another task places a non-face based family instance at a ceiling. Therefore the ceiling is selected and its height is passed to the family instance as its Revit level offset and also as a family parameter that controls the family's geometry. This family models the detection pattern for a specific manufacturer's lighting control occupancy sensor. In this example the manufacturer's published detection pattern data is contradictory and incomplete, so this family needs to be corrected. The concept here is to provide a design tool that allows the lighting control engineer to make an informed decision regarding sensor selection and placement. The family instance is not the sensor but is the detection pattern. It would be disposed or retained. It is placed on a unique workset regardless of the current workset.

The floating heads up WPF status device seen during the task is actually the same device used in every instance it is seen. WPF's ability to handle itself makes this possible.

Every "blade" in the detection pattern family is the same nested family. There are about five levels of nesting. In the last level there are three polar arrays. Without the arrays you would see three blades, each a different size. Parameters control each of the three ways the blades can rotate. One way to describe this is to say the blades can flap up and down like an umbrella, sweep side to side like windshield wipers or twist on their long axis like the pitch to a propeller. Since all of the angles are constant for the sensor detection data the only geometry change this family does is calculate the blade length so that the major blade touches the floor.
