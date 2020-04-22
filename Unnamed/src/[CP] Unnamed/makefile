.PHONY: clean dirs sdve json cleanup copy all

DIR=$${PWD\#\#*/}

clean:
	@echo "(`echo ${DIR}`): Cleaning..."; \
	rm -rf ./bin; \

dirs:
	@echo "(`echo ${DIR}`): Creating directories..."; \
	mkdir -p ./bin/tmp/sdvc; \
	mkdir -p ./bin/tmp/jsonc; \

sdve:
	@echo "(`echo ${DIR}`): Copying assets to temporary directory..."; \
	mkdir -p ./bin/tmp/sdvc/`echo ./**/`; \
	cp -r ./assets/* ./bin/tmp/sdvc/assets; \
	cp ./content.* ./bin/tmp/sdvc/; \
	cp ./manifest.* ./bin/tmp/sdvc/; \
	echo "(`echo ${DIR}`): Compiling Stardew Valley events..."; \
	./tools/sdvc ./bin --auto -O ./bin/tmp/sdvc; \
	rm `ls ./bin/tmp/sdvc/**/**/**/* | grep -o .*sdve.json`; \
	rm `ls ./bin/tmp/sdvc/**/**/**/* | grep -o .*sdve.targets.json`; \

json:
	@echo "(`echo ${DIR}`): Copying assets to temporary directory..."; \
	cp -r ./bin/tmp/sdvc/* ./bin/tmp/jsonc/; \
	echo "(`echo ${DIR}`): Compiling JSON..."; \
	./tools/jsonc ./bin/tmp/jsonc/**/*.json; \
	./tools/jsonc ./bin/tmp/jsonc/**/*.jsonc; \
	rm -rf ./bin/tmp/jsonc/*.jsonc; \
	rm `ls ./bin/tmp/jsonc/**/**/**/* | grep -o .*sdve`; \

copy:
	@echo "(`echo ${DIR}`): Copying files from temporary directory..."; \
	mkdir "./bin/[CP] Unnamed"
	cp -r ./bin/tmp/jsonc/* "./bin/[CP] Unnamed"

cleanup:
	@echo "(`echo ${DIR}`): Cleaning up..."; \
	rm -rf ./bin/tmp; \

all:
	@echo "(`echo ${DIR}`): Compiling Content Patcher mod..."; \
	make clean; \
	make dirs; \
	make sdve; \
	make json; \
	make copy; \
	make cleanup; \
	echo "(`echo ${DIR}`): Done!"; \