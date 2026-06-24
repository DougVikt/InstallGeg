.PHONY: build run clean test lint

build:
	@echo "=== Compilando o projeto em modo release ==="
	cargo build --release

run:
	@echo "=== Executando o projeto ==="
	cargo run --release

clean:
	@echo "=== Limpando artefatos de build ==="
	cargo clean

test:
	@echo "=== Executando testes ==="
	cargo test

lint:
	@echo "=== Verificando codigo com Clippy ==="
	cargo clippy -- -D warnings
