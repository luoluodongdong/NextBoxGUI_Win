#! coding=utf-8
import math

print("\n包含制定字符串:")
print("YES") if "th" in "thomas" else print("NO")


print("\n比较相同字符串:")
print("YES") if "ITKS" == "ITKS" or "123" == "456" else print("NO")

print("\n数学计算式:")
print(float("2") *(float("2")+1.1))

print("\n比较数值:")
print("YES") if float("12.12") > 12.11 else print("NO")

print("\n转换十六进制17:")
print(hex(int("17")))

print("\n转换二进制3:")
print(bin(int("3"))[2:])

print("\n按位与(2&3):")
print(bin(int("2")&int("3"))[2:])

print("\n数学计算圆周率:")
print(math.pi)

print("\n数学计算4的阶乘:")
print(math.factorial(int("4")))