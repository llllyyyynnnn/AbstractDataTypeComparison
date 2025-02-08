# Abstract Data Type Comparison
The purpose of the code is to compare the following types: List, SortedList, Dictionary, SortedDictionary and BinarySearchTree 
by giving them large sample data then returning information about how often certain words had appeared 
in the provided example as well as what word was the most re-occuring alongside the amount of unique words detected.

For the results to be accurate, the following was done.

* The file is read and assigned to a string array at the start of execution.
* The application waits a second between relevant operations to prevent any previous functions from affecting the other.
* The variables are defined within the functions themselves and are not passed on. 
* Stopwatch and CPU time are both being taken during the functions execution.
* Words checked increase by 10,000 at a time in order to also document how different word counts affect the effectivity of a certain structure.

Results will get saved into a .csv file for comparison, including all the information above.