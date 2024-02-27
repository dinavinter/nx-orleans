# <a id="grains_Contract_ICalculatorGrain"></a> Interface ICalculatorGrain

Namespace: [grains.Contract](grains.Contract.md)  
Assembly: grains.dll

```csharp
public interface ICalculatorGrain : IGrainWithGuidKey, IGrain, IAddressable
```

#### Implements

[IGrainWithGuidKey](https://learn.microsoft.com/dotnet/api/orleans.igrainwithguidkey),
[IGrain](https://learn.microsoft.com/dotnet/api/orleans.igrain),
[IAddressable](https://learn.microsoft.com/dotnet/api/orleans.runtime.iaddressable)

## Methods

### <a id="grains_Contract_ICalculatorGrain_Add_System_Int32_System_Int32_"></a> Add\(int, int\)

Adds two integers

```csharp
Task<int> Add(int l, int r)
```

#### Parameters

`l` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Integer to Add

`r` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Integer to Add

#### Returns

[Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

Sum of <see param="l"></see> and <see param="r"></see>
