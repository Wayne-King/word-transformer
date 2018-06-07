$Host.UI.RawUI.WindowTitle = 'Create Perf Counters'

[string] $category = 'Word Transformer'

if ([Diagnostics.PerformanceCounterCategory]::Exists($category))
{
    [Diagnostics.PerformanceCounterCategory]::Delete($category)
    Write-Host 'Deleted the existing category; waiting a moment before continuing ...'
    Start-Sleep -Seconds 3
}

$ccdc = [Diagnostics.CounterCreationDataCollection]::new()

# for TransformationTraversalWithMonitor
#
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'Traversal Backlog',
    'Average over time of the ratio of traversal embarks to disembarks.',
    'CountPerTimeInterval32'))
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'Traversal Embark per Second',
    'Average per second of the number of words added to the backlog.',
    'RateOfCountsPerSecond32'))
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'Traversal Disembark per Second',
    'Average per second of the number of words removed from the backlog.',
    'RateOfCountsPerSecond32'))

# for WordDictionaryWithMonitor
#
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'Word Hit Ratio',
    'Current ratio of text modulations that are valid words.',
    'SampleFraction'))
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'Word Hit Ratio - Base',
    'Current count of text modulations.',
    'SampleBase'))

# for WordTransformerWithMonitor
#
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'New Words per Word',
    'Average count of new words generated from each existing word.',
    'AverageCount64'))
$ccdc.Add([Diagnostics.CounterCreationData]::new(
    'New Words per Word - Base',
    'Number of existing words examined.',
    'AverageBase'))


[Diagnostics.PerformanceCounterCategory]::Create(
    $category,
    'Counters published by the Word Transformer application.',
    [Diagnostics.PerformanceCounterCategoryType]::SingleInstance,
    $ccdc)
