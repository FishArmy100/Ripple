# Itterators:
### IItterator Interface 
```swift
interface IItterator<TItem>
{
  func next(this) -> Option<TItem>;
}
```
### Enumerable Interface???:
```swift
interface IEnumerable<TItem>
{
  func to_iterator(this) -> impl IIterator<TItem>;
  func to_iterator(this&) -> impl IIterator<TItem>;
  func to_iterator(this mut&) -> impl IITerator<TItem>;
}
```

### Yeild Syntax
```swift
func fib_iter(usize count) -> impl IIterator<usize>
{
  usize mut i = count;
  if (i > 0)
    yeild 0;
    
  i--;
  if (i > 0)
    yeild 0;
    
  i--;
  usize mut fib = 1;
  
  for i in range(0...i)
  {
    fib += fib;
    yeild fib;
  }
}

func main() -> void
{
  for fib in fib_iter(50)
  {
    println("{}", fib);
  }
}
```
