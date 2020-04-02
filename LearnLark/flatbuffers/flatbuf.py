from lark import Lark

gf = open("flatbuf.lark", "r")
glark = gf.read()
gf.close()

parser = Lark(glark)

lf = open("monster.fbs", "r")
lang = lf.read()
lf.close()

print(parser.parse(lang).pretty())
