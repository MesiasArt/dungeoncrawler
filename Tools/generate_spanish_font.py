"""Extend the user-provided Thaleah TTF; preserves original Latin glyphs/metrics.
Dependency: fonttools (local Library/CodeboundFontTools or standard Python environment).
"""
from pathlib import Path
import sys
ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/'Library/CodeboundFontTools'))
from fontTools.ttLib import TTFont
from fontTools.pens.ttGlyphPen import TTGlyphPen
font=TTFont(ROOT/'Assets/Resources/UI/Fonts/ThaleahFat_TTF.ttf')
cmap=font.getBestCmap();glyphs=font['glyf'];order=list(font.getGlyphOrder())
def box(pen,x,y,w,h):
    pen.moveTo((x,y));pen.lineTo((x,y+h));pen.lineTo((x+w,y+h));pen.lineTo((x+w,y));pen.closePath()
def add(cp,base,mark=None,invert=False):
    name='uni%04X'%cp;base_name=cmap[ord(base)];g=glyphs[base_name];pen=TTGlyphPen(font.getGlyphSet())
    if invert:pen.addComponent(base_name,(-1,0,0,-1,g.xMax,448))
    else:
        pen.addComponent(base_name,(1,0,0,1,0,0));cx=(g.xMin+g.xMax)//2
        if mark=='acute':box(pen,cx-64,512,64,64);box(pen,cx,576,64,64)
        elif mark=='diaeresis':box(pen,cx-128,512,64,64);box(pen,cx+64,512,64,64)
        elif mark=='tilde':
            for x,y,w,h in [(cx-160,512,64,64),(cx-96,576,128,64),(cx+32,512,128,64),(cx+160,576,64,64)]:box(pen,x,y,w,h)
    glyphs[name]=pen.glyph();font['hmtx'].metrics[name]=font['hmtx'].metrics[base_name];order.append(name)
    for table in font['cmap'].tables:
        if table.isUnicode():table.cmap[cp]=name
for cp,base in [(0xC1,'A'),(0xC9,'E'),(0xCD,'I'),(0xD3,'O'),(0xDA,'U'),(0xE1,'a'),(0xE9,'e'),(0xED,'i'),(0xF3,'o'),(0xFA,'u')]:add(cp,base,'acute')
for cp,base in [(0xD1,'N'),(0xF1,'n')]:add(cp,base,'tilde')
for cp,base in [(0xDC,'U'),(0xFC,'u')]:add(cp,base,'diaeresis')
add(0xBF,'?',invert=True)
# Existing inverted exclamation is retained. Additional common UI punctuation.
for cp,width in [(0x2013,448),(0x2014,704),(0x2026,512)]:
    name='uni%04X'%cp;pen=TTGlyphPen(None)
    if cp==0x2026:
        for x in [0,192,384]:box(pen,x,0,64,64)
    else:box(pen,0,192,width-64,64)
    glyphs[name]=pen.glyph();font['hmtx'].metrics[name]=(width,0);order.append(name)
    for table in font['cmap'].tables:
        if table.isUnicode():table.cmap[cp]=name
font.setGlyphOrder(order)
for name_id,value in [(1,'Codebound Pixel Spanish'),(2,'Regular'),(3,'CodeboundPixelSpanish-1.0'),(4,'Codebound Pixel Spanish'),(6,'CodeboundPixelSpanish')]:
    font['name'].setName(value,name_id,3,1,0x409)
font.save(ROOT/'Assets/Resources/UI/Fonts/CodeboundPixelSpanish.ttf')
print('Generated Spanish font; original unchanged.')
