# Add your Python code here. E.g.
from microbit import *

mode = 0

pin0.write_digital(0)
pin1.write_digital(0)
pin2.write_digital(0)
pin8.write_digital(0)

display.show(Image.NO)

def avancer():
    pin0.write_digital(1)
    pin1.write_digital(0)
    pin2.write_digital(0)
    pin8.write_digital(1)
    sleep(100)
    pin0.write_digital(0)
    pin1.write_digital(0)
    pin2.write_digital(1)
    pin8.write_digital(1)
    sleep(100)
    pin0.write_digital(0)
    pin1.write_digital(1)
    pin2.write_digital(1)
    pin8.write_digital(0)
    sleep(100)
    pin0.write_digital(1)
    pin1.write_digital(1)
    pin2.write_digital(0)
    pin8.write_digital(0)
    sleep(100)


while True:
    
    if mode == 1:
        avancer()
    
    if button_a.was_pressed():
        if mode == 0:
            mode = 1
            display.show(Image.YES)
        elif mode == 1:
            mode = 0
            display.show(Image.NO)
