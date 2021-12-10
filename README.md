# DeclarativePM

## User Interface

### Examples
Example log can be found in sampleData directory under name `webstore.csv`
Example fitting declare model can be found in sampleData directory under name `webstore.json`
Example non fitting declare model can be found in sampleData directory under name `webstore_bad.json`

### Import / Export
You can import or export models on `Import/Export` tab. To save imported Log do not forget to click Save

### Discovery
You can discover declare models from the log on `Discover` tab. In order to discover you have to have at least one log imported. 
Later it is necessary to choose templates which you want to discover. In next step you can configure templates further.
Last step is discovery, you can check discovered model and save it, or go back and edit settings for discovery.

### Create / edit
On tab `Create model` you can either create completely new declare model from scratch by choosing `Create declare model`, a new declare model using activities from some log choosing `create declare model from log`, or edit already existing model by choosing `edit existing model`

### Conformance
On tab `Conformance` you can check conformance of a trace with respect to some declare model. You need to have at least one declare model already imported or created.

On the right side of screen you can see currently chosen declare model. In the right upper corner choose `Add/change model` to add model for conformance or chage chosen for a new one. 

On the left you can observe and choose traces for conformance checking, or remove them. In the left upper corner you can click `Add trace` to add new trace, which you need for conformance. You can choose from 2 options. You can either use a trace from already existing log, or you can create completely new trace. If you choose to create new trace, you can click `See activities` in the left upper corner to add new activities which you can use to create new traces. Click again on `See traces` to see traces again.

Middle part of the screen server for interaction. You can click `See trace` to see currently chosen trace and to remove events from it.

Once you chosen trace on the left by clicking at it, and you have already chosed declare model, you can click `Evaluate` to evaluate the trace.
You will see diferet statistics or you can choose from different templates and its instances and see where they have been activated, violated or conflicted in the trace.

Purple - activation
Yellow - conflict
Red - violation

## Library

//TODO

Discovery.cs for discovery

ActivationBinaryTree.cs for conformance checking
ActivationTreeBuilder.cs to build ActivationBinaryTree from the trace and constraint

//TODO divide into files and folders
MainMethods.cs for main methods like trace and expression evaluation, but also evaluation for conformance

#Declare Templates

ITemplate.cs main interface for each template
IVacuityDetection.cs for templates could be checked with vacuity condition

ExistenceTemplate.cs, UniTemplate.cs and BiTemplate are abstract classes for different categories of templates

BiTemplateFactory.cs, UniTemplateFactory.cs and ExistenceFactory.cs are factories for creation of template instances. Mainly generation of candidates.

The rest are classes for specific templates implementing abstract classses and interfaces defined above.

### LtlExpression

LtlExpression.cs is recursive class representing expressions from linear temporal logic. For now they can be either:
  - atomic, consisting of None operator and Atom as atomic expression 
  - unary, consisting of unary operator and one subexpression on the left
  - binary, consiting of binary operator and two subexpressions on the left and right

### Parametrized template

Parametrized template is wrapping template, its parameters, Template description, types and list of template instances - constraints.

### Template Description

`TemplateDescription.cs` holds information about templates, such as their readable name, descriptio which can be printed, types, categories, etc.

### IO

For import you can use class `Importer.cs` which can either load csv logs from stream or local file path. It can also load and deserialize Declare models from .json

For export you can use class `Exporter.cs` which can serialize export declare models either into string, or save directly into provided directory.


