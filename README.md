# Norns

## Goal

This a project to do static weaving and dynamic weaving.

## Desgin

### AOP base on proxy

0. Generate proxy class type
1. Replace type to proxy type for di
2. Do aop in proxy class

### Static weaving generate code base on [roslyn](https://github.com/dotnet/roslyn)

There is two way that we will try to support :

#### AOT

* use [source-generators](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.md) to generator proxy class code 

#### JIT

* use Reflection to generator proxy class code 
* use roslyn sdk to convert code to type

### Dynamic weaving

* Emit to generate proxy type

> (ps: this will begin after static weaving done)

## 取名缘由

Norns == 诺伦三女神 （北欧神话中的命运女神）

大女儿乌尔德（Urd）司掌“过去”，二女儿薇儿丹蒂（Verthandi）司掌“现在”，小女儿诗蔻蒂（Skuld）司掌“未来”。

所以借用该名希望该项目可以帮助大家 `自我掌控` 项目的“命运”。
