# <a id="grains_Implementation_CalculatorGrain"></a> Class CalculatorGrain

Namespace: [grains.Implementation](grains.Implementation.md)  
Assembly: grains.dll

```csharp
public class CalculatorGrain : Grain, ILifecycleParticipant<IGrainLifecycle>, ICalculatorGrain, IGrainWithGuidKey, IGrain, IAddressable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ←
[Grain](https://learn.microsoft.com/dotnet/api/orleans.grain) ←
[CalculatorGrain](grains.Implementation.CalculatorGrain.md)

#### Implements

[ILifecycleParticipant<IGrainLifecycle\>](https://learn.microsoft.com/dotnet/api/orleans.ilifecycleparticipant-1),
[ICalculatorGrain](grains.Contract.ICalculatorGrain.md),
[IGrainWithGuidKey](https://learn.microsoft.com/dotnet/api/orleans.igrainwithguidkey),
[IGrain](https://learn.microsoft.com/dotnet/api/orleans.igrain),
[IAddressable](https://learn.microsoft.com/dotnet/api/orleans.runtime.iaddressable)

#### Inherited Members

[Grain.RegisterTimer\(Func<object, Task\>, object, TimeSpan, TimeSpan\)](https://learn.microsoft.com/dotnet/api/orleans.grain.registertimer),
[Grain.RegisterOrUpdateReminder\(string, TimeSpan, TimeSpan\)](https://learn.microsoft.com/dotnet/api/orleans.grain.registerorupdatereminder),
[Grain.UnregisterReminder\(IGrainReminder\)](https://learn.microsoft.com/dotnet/api/orleans.grain.unregisterreminder),
[Grain.GetReminder\(string\)](https://learn.microsoft.com/dotnet/api/orleans.grain.getreminder),
[Grain.GetReminders\(\)](https://learn.microsoft.com/dotnet/api/orleans.grain.getreminders),
[Grain.GetStreamProvider\(string\)](https://learn.microsoft.com/dotnet/api/orleans.grain.getstreamprovider),
[Grain.DeactivateOnIdle\(\)](https://learn.microsoft.com/dotnet/api/orleans.grain.deactivateonidle),
[Grain.DelayDeactivation\(TimeSpan\)](https://learn.microsoft.com/dotnet/api/orleans.grain.delaydeactivation),
[Grain.OnActivateAsync\(\)](https://learn.microsoft.com/dotnet/api/orleans.grain.onactivateasync),
[Grain.OnDeactivateAsync\(\)](https://learn.microsoft.com/dotnet/api/orleans.grain.ondeactivateasync),
[Grain.Participate\(IGrainLifecycle\)](https://learn.microsoft.com/dotnet/api/orleans.grain.participate),
[Grain.GrainReference](https://learn.microsoft.com/dotnet/api/orleans.grain.grainreference),
[Grain.GrainFactory](https://learn.microsoft.com/dotnet/api/orleans.grain.grainfactory),
[Grain.ServiceProvider](https://learn.microsoft.com/dotnet/api/orleans.grain.serviceprovider),
[Grain.IdentityString](https://learn.microsoft.com/dotnet/api/orleans.grain.identitystring),
[Grain.RuntimeIdentity](https://learn.microsoft.com/dotnet/api/orleans.grain.runtimeidentity),
[object.Equals\(object?\)](<https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)>),
[object.Equals\(object?, object?\)](<https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)>),
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode),
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype),
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone),
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals),
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="grains_Implementation_CalculatorGrain_Add_System_Int32_System_Int32_"></a> Add\(int, int\)

Adds two integers

```csharp
public Task<int> Add(int l, int r)
```

#### Parameters

`l` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Integer to Add

`r` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Integer to Add

#### Returns

[Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

Sum of <see param="l"></see> and <see param="r"></see>
