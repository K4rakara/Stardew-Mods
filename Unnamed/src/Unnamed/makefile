.PHONY: clean cs copy all

clean:
	@rm -rf ./bin; \

cs:
	@msbuild; \

copy: 
	@echo "Copying compiled files..."; \
	mkdir ./bin/Unnamed/; \
	cp ./bin/Debug/Unnamed.dll ./bin/Unnamed/; \
	cp ./bin/Debug/Unnamed.pdb ./bin/Unnamed/; \
	cp ./manifest.json ./bin/Unnamed/; \

all:
	@echo "Compiling Unnamed..."; \
	make clean; \
	make cs; \
	make copy; \