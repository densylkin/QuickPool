# Unity-QuickPool

Usage
-----
You can use quickpool in two ways:
- Create PoolManager and configure your pools with editor 

[Imgur](http://i.imgur.com/mMy5Pr5.png)

- Or create a Pool field (in this case you will need to register your pool somewhere in Start with PoolManager.RegisterPool())

[Imgur](http://i.imgur.com/N6jDhrr.png)

Include QuickPool namespace

```C#
using QuickPool;
```

You can use extention methods

```C#
var instance = somePrefab.Spawn(Vector3.zero, Quaternion.identity);
var component = somePrefab.Spawn<Component>(Vector3.zero, Quaternion.identity);
obj.Despawn();
```

You also can get concrete pool from PoolManager or use that one you have created

```C#
public Pool pool = new Pool()
{
  size = 100,
  allowGrowth = true
};

// OR

var pool = PoolManager.Instance["pool name"];
```
```C#
private void Start()
{
  PoolsManager.RegisterPool(pool); //register pool if you want to use extention method Despawn
  pool.Initialize(); 
  
  var instance = pool.Spawn(Vector3.zero, Quaternion.identity);
  var conponent = pool.Spawn<Component>(Vector3.zero, Quaternion.identity);
  pool.Despawn(instance);
}
```
Despawn all spawned objects

```C#
PoolsManager.DespawnAll();
// OR
pool.DespawnAll();
```

License
-----
[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US). You are free to use the QuickPool in any and all games that you make. You cannot sell the QuickPool directly or as part of a larger game asset.
