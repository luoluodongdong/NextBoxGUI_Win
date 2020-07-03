#! coding=utf-8
import argparse
import math

#print("hello")

def execute_cmd(cmd):
	try:
		#print(cmd)
		#exec(cmd)
		eval(str(cmd))
		print("[ACK]")
	except Exception as e:
		#print(e)
		print("[ERROR]")
	

if __name__ == '__main__':
	parser = argparse.ArgumentParser()
	parser.add_argument('-c', '--commands', help='python commands will be execute', type=str, default=None)
	args = parser.parse_args()

	execute_cmd(args.commands)