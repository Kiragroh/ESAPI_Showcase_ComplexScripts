# ESAPI_Showcase-ComplexScripts

Screenshots of my often used complex ESAPI-GUI-Scripts. These screenshots should inspire you for your own projects.

Questions at	m.grohmann@gmx.net.

Why do I not share the source code?:
- code is not user friendly because I build these scripts iterative with less time. would be much work to make them universally readable
- many obvious and hidden features are implemented that would need a manual
- in some parts/checks the scripts are clinic specific
- fear that wrong usage or integration will lead to wrong clinical decisions

0.) PlanCheck
- check DoseConstraints and plan parameters for plans or planSums
- generate Reports with interactive DVH from GUI
- script structure is adopted from LDClark (https://github.com/LDClark/PlanCheck) but changed a lot 
![GIF 1](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/PlanCheckWithDVH.gif)

1.) ContouringAssistant
- adapted script from https://github.com/mtparagon5/ESAPI-Projects/tree/master/Projects/v15/OptiAssistant
![Test Image 1](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/AutoContouring_newFeature_boolean.png)

2.) AutoPlan-GUI
- Autoplanning with GUI for User-input. More autoplanning features can easily be added.
![Test Image 2](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/AutoPlan-GUI.PNG)

3.) STX-CalculateIndices-GUI
- calculate Paddick-CI & -GI in Eclipse without HyperArc-License for single or multiple targets in plans or plansums.
![Test Image 3](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/STX-Check-GUI.PNG)

4.) QA-Exporter
- Standalone script to quickly export RP-files to Third-Party-QA programs
- before and after exporting, additional tests will be made (Imager for PD activated, Gating deactivaded, etc.)
- DICOM-Export functionality can be added to ESAPI via an extra DICOM-Daemon (Tutorial: https://github.com/VarianAPIs/Varian-Code-Samples/wiki/Scripting-the-Varian-DICOM-DB-Daemon-with-ESAPI -> Code has to be changed a little bit to work in standalone programs and for more modalities)
![Test Image 8](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/QA-Exporter.png)

5.) Eclipse-DataMiner
- aquire data of specific patients or all or all but filtered
- inspired by the GUI-structure from https://github.com/tkmd94/EclipseDataMiner but the output and features are mostly changed
- DICOM-Export functionality can be added to ESAPI via an extra DICOM-Daemon (Tutorial: https://github.com/VarianAPIs/Varian-Code-Samples/wiki/Scripting-the-Varian-DICOM-DB-Daemon-with-ESAPI -> Code has to be changed a little bit to work in standalone programs and for more modalities)
![Test Image 9](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner6.PNG)
![Test Image 4](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner.png)
![Test Image 5](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner2.png)
![Test Image 6](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner3.png)
- example output for one plan (real outout is in .csv-format)
![Test Image 7](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner4.png)
![Test Image 8](https://github.com/Kiragroh/ESAPI_Showcase_ComplexScripts/blob/master/EclipseDataMiner5.png)


