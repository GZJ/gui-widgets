from setuptools import setup, find_packages

setup(
    name='gui-widgets',
    version='0.1.0',
    description='some gui widgets',
    author='gzj',
    author_email='gzj00@outlook.com',
    url='https://github.com/gzj/gui-widgets',
    packages=find_packages(where='.', include=['*'], exclude=['*.*']),
    scripts=['gk-list.py', 'gk-finput.py', 'gk-fuzzy-search'],
    install_requires=['PyQt5'],
)
